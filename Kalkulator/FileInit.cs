using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Kalkulator
{
    public static class FileInit
    {
        private static string m_CostPath, m_MaterialPath;
        private static List<Material> material = new List<Material>();
        private static List<Cutting> cuttings = new List<Cutting>();

        internal static List<Material> Material { get => material; set => material = value; }
        internal static List<Cutting> Cuttings { get => cuttings; set => cuttings = value; }

        public static void Init(string costPath, string materialPath)
        {
            m_CostPath = costPath;
            m_MaterialPath = materialPath;
            try
            {
                ReadMaterial();
                ReadCost();
            }
            catch (Exception ex)
            {
                throw new Exception("Nie można załadować plików konfiguracyjnych " + ex.Message);
            }
        }

        public static float GetCostByType(string type, float thickness)
        {
            float cost = 0;
            foreach (Cutting item in Cuttings)
            {
                if (item.Thickness == thickness)
                {
                    switch (type)
                    {
                        case "Stainless":
                            return item.Stainless;

                        case "Building":
                            return item.Building;

                        case "Premium":
                            return item.Premium;

                        default:
                            break;
                    }
                }
            }
            return cost;

        }

        private static void ReadMaterial()
        {
            XDocument document = XDocument.Load(m_MaterialPath);
            int i = 0;
            var q = from n in document.Descendants("Material")
                    select new
                    {
                        Density = (float)Convert.ToDouble(n.Element("Density").Value),
                        Price = (float)Convert.ToDouble(n.Element("Price").Value),
                        Type = (string)n.Element("Type").Value
                    };

            foreach (XElement el in document.Root.Elements())
            {
                if (el.Name == "Material")
                {
                    Material.Add(new Material());
                    Material[i].MaterialName = el.Attribute("name").Value;
                    i++;
                }
            }

            i = 0;
            foreach (var p in q)
            {
                Material[i].Density = p.Density;
                Material[i].MaterialPrice = p.Price;
                Material[i].MaterialType = p.Type;
                i++;
            }

        }

        private static void ReadCost()
        {
            XDocument document = XDocument.Load(m_CostPath);

            var Materials = (from n in document.Descendants("Thickness")
                             let children = n.Elements("Type")
                             select new Cutting()
                             {
                                 Stainless = float.Parse(children.First(tag=>tag.Attribute("name").Value == "Stainless").Value),
                                 Building = float.Parse(children.First(tag => tag.Attribute("name").Value == "Building").Value),
                                 Premium = float.Parse(children.First(tag => tag.Attribute("name").Value == "Premium").Value),
                             }).ToList();

            int i = 0;
            int ile = document.Descendants("Thickness").Count();
            for (i = 0; i < ile; i++)
            {
                cuttings.Add(new Cutting());
                cuttings[i].Stainless = Materials[i].Stainless;
                cuttings[i].Building = Materials[i].Building;
                cuttings[i].Premium = Materials[i].Premium;
            }

            i = 0;
            foreach (XElement element in document.Root.Elements())
            {
                if (element.Name == "Thickness")
                {
                    cuttings[i].Thickness = (float)Convert.ToDouble(element.Attribute("name").Value);
                    i++;
                }
            }
        }
    }
}
