using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingRequisition.Assessments.Classes
{
    [Serializable]
    public class Rating
    {
        public int Value { get; set; }

        public static List<Rating> GetAll()
        {
            List<Rating> output = new List<Rating>();
            return output;
        }
    }
}
