using System;

namespace shoppingCartLib
{
    public class Coupon
    {
        private string code;
        private DateTime startDate;
        private DateTime endDate;
        private double discountPercentage;
        private string applicableCategory;
        private string applicableItemCode;
        private bool isValid;

        public string Code
        {
            get
            {
                return code;
            }

            set
            {
                code = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return startDate;
            }

            set
            {
                startDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return endDate;
            }

            set
            {
                endDate = value;
            }
        }

        public double DiscountPercentage
        {
            get
            {
                return discountPercentage;
            }

            set
            {
                if (value >= 0 && value < 100)
                {
                    discountPercentage = value;
                }
                else
                {
                    discountPercentage = 100;
                }
            }
        }

        public string ApplicableCategory
        {
            get
            {
                return applicableCategory;
            }

            set
            {
                applicableCategory = value;
            }
        }

        public string ApplicableItemCode
        {
            get
            {
                return applicableItemCode;
            }

            set
            {
                applicableItemCode = value;
            }
        }

        public bool IsValid
        {
            get
            {
                DateTime dateToday = DateTime.Now;
                isValid = dateToday.Ticks > StartDate.Ticks && dateToday.Ticks < EndDate.Ticks; 
                return isValid;
            }
        }

        public Coupon(string code, DateTime d1, DateTime d2, double dis, string applicableCategory,string applicableItemCode) {
            this.Code = code;
            this.StartDate = d1;
            this.EndDate = d2;
            this.DiscountPercentage = dis;
            this.ApplicableCategory = applicableCategory;
            this.ApplicableItemCode = applicableItemCode;
        }


        public override string ToString()
        {
            string message = "";
            if (!IsValid)
            {
                message += "Coupon Code: " +
        Code + "\nStart Date: " + StartDate + "\nStart Date: "
        + EndDate+"\n";

                message += "It's invalid now!";
            }
            else
            {
                // string message
                message = "Coupon Code: " +
                        Code + "\n";

                if (!string.IsNullOrEmpty(ApplicableCategory))
                {
                    message += "Applicable Category:" +
                        ApplicableCategory +
                        "\nDiscount: " + DiscountPercentage + "%";
                }
                else if (!string.IsNullOrEmpty(ApplicableItemCode))
                {
                    // Item item = SearchItemById(ApplicableItemCode);
                    message += "Applicable Item:" +
                        ApplicableItemCode +
                        "\nDiscount: " + DiscountPercentage + "%";
                }
            }

            
            return message;
        }

    }
}
