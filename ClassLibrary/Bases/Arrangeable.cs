using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TrainingRequisition.ClassLibrary.Bases
{
    [Serializable]
    public abstract class Arrangeable: Base
    {
        public int DisplayOrder { get; set; }

        public virtual void LoadFromReader(SqlDataReader dr)
        {
            base.LoadFromReader(dr);
            DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
        }

        public virtual void Copy(Base source)
        {
            base.Copy(source);
            Arrangeable src = (Arrangeable)source;
            DisplayOrder = src.DisplayOrder;
        }

        public override void Save(System.Data.DataRow row)
        {
            row["DisplayOrder"] = DisplayOrder;
            base.Save(row);
        }
    }
}
