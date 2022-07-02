namespace AStar.Common.Generators
{
    public interface IMatrixGenerator
    {
        public Matrix Generate(float passablePercent, (int, int) size);
    }
}