using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunkFood.AppData
{
   
    public class Employee
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Имя")]
        public string first_name { get; set; }
        [DisplayName("Фамилия")]
        public string last_name { get; set; }
        [DisplayName("Отчество")]
        public string Patronymic { get; set; }
        [DisplayName("Телефон")]
        public string Phone { get; set; }
        [DisplayName("День рождения")]
        public DateTime Birthday { get; set; }
        [DisplayName("Поступление на работу")]
        public DateTime Start_date { get; set; }
        [DisplayName("Должность")]
        public int Post_id { get; set; } // Foreign Key
    }

    public class Order
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Имя")]
        public string HowCall { get; set; }
        [DisplayName("Время заказа")]
        public DateTime DateTime { get; set; }
        [DisplayName("Комментарий")]
        public string Note { get; set; }
        [DisplayName("Доставка")]
        public bool ForDelivery { get; set; }
        [DisplayName("Статус")]
        public Status Status { get; set; }
        [DisplayName("Сумма")]
        public decimal Sum { get; set; }
        [DisplayName("Стол")]
        public int TableNumber { get; set; }
    }

    public class MenuItem
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; }
        [DisplayName("Описание")]
        public string Description { get; set; } // Will be stored as JSON
        [DisplayName("Цена")]
        public decimal Price { get; set; }
    }

    public class MenuIngredient
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; }
        [DisplayName("Количество")]
        public decimal Unit { get; set; }
    }
}
