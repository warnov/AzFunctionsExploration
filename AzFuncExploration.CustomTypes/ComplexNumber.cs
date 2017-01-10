using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes
{
    public class ComplexNumber
    {

        public double Real { get; set; }
        public double Imaginary { get; set; }

        public ComplexNumber()
        {
            Real = 0d;
            Imaginary = 0d;
        }

        public ComplexNumber(double real, double imaginary = 0)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public ComplexNumber Sum(ComplexNumber a, ComplexNumber b)
        {
            return new ComplexNumber()
            {
                Real = a.Real + b.Real,
                Imaginary = a.Imaginary + b.Imaginary
            };
        }

        public override string ToString()
        {
            return $"{Real}+{Imaginary}i";
        }
    }
}