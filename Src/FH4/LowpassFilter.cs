//
// Copyright (c) 2019 Rausch IT
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
//
//
namespace FH4
{
    class LowpassFilter
    {
        double Yp;
        double Ypp;
        double Yppp;
        double Xp;
        double Xpp;

        public double firstOrder_lowpassFilter(double X, double beta)
        {
            double Y;

            Y = beta * X + (1 - beta) * Yp;
            Yp = Y;

            return Y;
        }

        public double secondOrder_lowpassFilter(double X, double beta)
        {
            double Y;

            Y = beta * X + beta * (1 - beta) * Xp + (1 - beta) * (1 - beta) * Ypp;

            Xp = X;
            Ypp = Yp;
            Yp = Y;

            return Y;
        }

        public double thirdOrder_lowpassFilter(double X, double beta)
        {
            double Y;

            Y = beta * X + beta * (1 - beta) * Xp + beta * (1 - beta) * (1 - beta) * Xpp + (1 - beta) * (1 - beta) * (1 - beta) * Yppp;

            Xpp = Xp;
            Xp = X;
            Yppp = Ypp;
            Ypp = Yp;
            Yp = Y;

            return Y;
        }

        public void Clear()
        {
            Yp = 0;
            Ypp = 0;
            Yppp = 0;
            Xp = 0;
            Xpp = 0;
        }

    }
}
