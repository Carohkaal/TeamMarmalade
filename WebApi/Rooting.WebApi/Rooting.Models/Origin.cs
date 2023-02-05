namespace Rooting.Models
{
    public class Origin : IOrigin
    {
        public Origin(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; set; }
        public int Col { get; set; }
    }
}