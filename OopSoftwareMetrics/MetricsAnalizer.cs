using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OopSoftwareMetrics
{
    static class MetricsAnalizer
    {
        private static readonly Object _thisLock = new Object();

        private static int GetDIT(Assembly assembly)
        {
            var DIT = 0;

            foreach (Type type in assembly.GetTypes())
            {
                var depth = 0;
                for (var current = type.BaseType; current != null; current = current.BaseType, depth++);

                DIT = Math.Max(DIT, depth);
            }

            return DIT;
        }

        private static int GetNOC(Assembly assembly)
        {
            var NOC = 0;
            var assemblyTypes = assembly.GetTypes();

            foreach (Type type in assemblyTypes)
            {
                NOC = Math.Max(NOC, assemblyTypes.Count(t => type.Equals(t.BaseType)));
            }

            return NOC;
        }

        private static decimal GetMHF(Assembly assembly)
        {
            /*
                Mv – количество видимых методов в классе, 
                Mh – количество скрытых методов класса,
            */
            int Mv = 0;
            int Mh = 0;

            foreach (Type type in assembly.GetTypes())
            {
                Mv += type.GetMethods(/*BindingFlags.DeclaredOnly*/).Count();
                Mh += type.GetMethods(/*BindingFlags.DeclaredOnly | */BindingFlags.NonPublic | BindingFlags.Instance |
                        BindingFlags.Static)
                    .Count();
            }

            var allMethods = Mh + Mv;

            return allMethods != 0 ? (decimal)Mh / allMethods : 0;
        }

        private static decimal GetAHF(Assembly assembly)
        {
            /*
                Ah - количество скрытых атрибутов класса (интерфейс класса), 
                Ad – общее количество атрибутов, определенных в классе (без учета унаследованных), 
            */
            int Ad = 0;
            int Ah = 0;

            foreach (Type type in assembly.GetTypes())
            {
                Ad += type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Static | BindingFlags.Instance)
                    .Count();
                Ah += type.GetFields(/*BindingFlags.DeclaredOnly | */BindingFlags.NonPublic | BindingFlags.Static |
                        BindingFlags.Instance)
                    .Count();
            }

            return Ad != 0 ? (decimal)Ah / Ad : 0;
        }

        private static decimal GetMIF(Assembly assembly)
        {
            /*
                Mi - количество унаследованых и не переопределенных методов класса,
                Ma - количество всех методов, доступных в классе.
            */
            int Ma = 0;
            int Mi = 0;

            foreach (Type type in assembly.GetTypes())
            {
                Ma += type.GetMethods(/*BindingFlags.DeclaredOnly | */BindingFlags.NonPublic | BindingFlags.Instance |
                        BindingFlags.Static | BindingFlags.Public)
                    .Count();
                Mi += type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.FlattenHierarchy)
                    .Where(m => m.DeclaringType != type && m.GetBaseDefinition() != m)
                    .Count();
            }

            return Ma != 0 ? (decimal)Mi / Ma : 0;
        }

        private static decimal GetAIF(Assembly assembly)
        {
            /*
                Ai- количество унаследованых и не переопределенных атрибутов класса, 
                Aa – общее количество атрибутов, определенных в классе,
            */
            int Aa = 0;
            int Ai = 0;

            foreach (Type type in assembly.GetTypes())
            {
                Aa += type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Static | BindingFlags.Instance)
                .Count();
                Ai += type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.FlattenHierarchy)
                    .Where(f => type.GetField(f.Name, BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static) == null)
                    .Count(); ;
            }

            return Aa != 0 ? (decimal)Ai / Aa : 0;
        }

        private static decimal GetPOF(Assembly assembly)
        {
            /*
                Mo -количество унаследованых и переопределенных методов класса,
                Mn -количество новых методов, доступных в классе,
                DC - количество потомков класса 
            */
            var Mo = 0;
            var Mn = 0;
            var DC = 0;
            var assemblyTypes = assembly.GetTypes();

            foreach (Type type in assemblyTypes)
            {
                Mo += type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.FlattenHierarchy)
                    .Where(m => m.GetBaseDefinition() != m && m.DeclaringType == type)
                    .Count();
                Mn += type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.DeclaredOnly)
                    .Where(m => m.GetBaseDefinition() == m)
                    .Count();
                DC += assemblyTypes.Count(t => type.Equals(t.BaseType));
            }

            return Mn == 0 || DC == 0 ? 0 : (decimal)Mo / (Mn * DC);
        }

        public static Metrics GetAssemblyMetrics(Assembly assembly)
        {
            return new Metrics
            {
                DIT = GetDIT(assembly),
                NOC = GetNOC(assembly),
                MHF = GetMHF(assembly),
                AHF = GetAHF(assembly),
                MIF = GetMIF(assembly),
                AIF = GetAIF(assembly),
                POF = GetPOF(assembly)
            };
        }
    }
}
