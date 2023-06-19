namespace ProjectWorlds
{
    [System.Serializable]
    public class Pair<T, U>
    {
        public T Key { get; set; } = default(T);
        public U Value { get; set; } = default(U);

        public Pair()
        {

        }

        public Pair(T key, U value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return "Pair<" + Key.GetType() + "," + Value.GetType() + ">(" + Key.ToString() + ", " + Value.ToString() + ")";
        }
    }
}
