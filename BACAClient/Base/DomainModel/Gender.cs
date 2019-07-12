using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel 
{
    public class Gender :BaseSelectItem<Gender>
    {
        public Gender()
        {}

        public Gender(int id,string name)
        {
            this.Id = id;
            this.DisplayName = name;
        }

        protected override IEnumerable<Gender> GetInnterList()
        {
            yield return new Gender(0, "女");
            yield return new Gender(1, "男");
        }
    }
}
