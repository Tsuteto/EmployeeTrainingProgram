using System.Collections.Generic;
using MyBox;

namespace EmployeeTraining
{
    public class ShoppingCustomerList : Singleton<ShoppingCustomerList>
    {
        public List<Customer> CustomersInShopping { get; private set; }

        public void Awake()
        {
            this.CustomersInShopping = new List<Customer>();
        }

        public void StartShopping(Customer c)
        {
            this.CustomersInShopping.Add(c);
        }

        public void FinishShopping(Customer c)
        {
            this.CustomersInShopping.Remove(c);
        }

        public void ClearCustomers()
        {
            this.CustomersInShopping.Clear();
        }
    }
}