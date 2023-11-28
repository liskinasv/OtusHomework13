namespace OtusHomework13
{
    public class F
    {
        public int i1, i2, i3, i4, i5;
        public double d2;

        public string cats { get; set; }
        public DateTime date { get; set; }

        public static F Get() => new F() { i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5, d2 = 0.22, cats = "Мурка, Барсик", date = DateTime.Now};

        public override string ToString()
        {
            string str = $"i1 = {i1}\ni2 = {i2}\ni3 = {i3}\ni4 = {i4}\ni5 = {i5}\nd2 = {d2}\ncat = {cats}\ndate = {date.ToString()}";

            return str;
        }
    }
}
