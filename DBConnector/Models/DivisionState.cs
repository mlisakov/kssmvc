using System;

namespace KSS.DBConnector.Models
{
    public class DivisionState
    {
        private Guid _id;
        private string _name;
        /// <summary>
        /// Идентификатор подразделения
        /// </summary>
        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        ///// <summary>
        ///// Краткое название
        ///// </summary>
        //public string Brief { get; set; }
        ///// <summary>
        ///// Родительское подразделение
        ///// </summary>
        //public Division Parent { get; set; }
    }
}