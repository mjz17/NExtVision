using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Plugin.BlobAnalysis
{
    public class Region_features
    {
        public HRegion SortRegion(HRegion in_Region, SortModel model, bool OrderModel)
        {
            return in_Region.SortRegion("character", "true", "row");
        }

        public List<Region_Info> Gne_Feature(HRegion in_Region)
        {
            List<Region_Info> infos = new List<Region_Info>();
            for (int i = 0; i < in_Region.Area.Length; i++)
            {
                infos.Add(new Region_Info()
                {
                    m_Area = in_Region.Area[i],
                    m_Row = in_Region.Row[i],
                    m_Column = in_Region.Column[i]
                });
            }
            return infos;
        }
    }

    public class Region_Info
    {

        /// <summary>
        /// 面积
        /// </summary>
        private double _Area;
        public double m_Area
        {
            get { return _Area; }
            set { _Area = value; }
        }

        /// <summary>
        /// Row
        /// </summary>
        private double _Row;
        public double m_Row
        {
            get { return _Row; }
            set { _Row = value; }
        }

        /// <summary>
        /// Colum
        /// </summary>
        private double _Column;
        public double m_Column
        {
            get { return _Column; }
            set { _Column = value; }
        }

    }

    public enum SortModel
    {
        面积,
        X坐标,
        Y坐标,
    }

}
