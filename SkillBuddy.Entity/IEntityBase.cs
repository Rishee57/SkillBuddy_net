using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SkillBuddy.Entity
{
    public interface IEntityBase
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }


        public void ParseDataRow(
            DataRow dRow,
            string colId = "Id",
            string colActive = "Active",
            string colCreatedOn = "CreatedOn",
            string colCreatedBy = "CreatedBy",
            string colCreatedByName = "CreatedByName",
            string colUpdatedOn = "UpdatedOn",
            string colUpdatedBy = "UpdatedBy",
            string colUpdatedByName = "UpdatedByName"

        );
    }
}