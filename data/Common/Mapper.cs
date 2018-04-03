namespace data.Common
{
    abstract class Mapper<E, T>
    {
        public abstract T MapFrom(E from);
    }
}
