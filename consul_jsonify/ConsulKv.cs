namespace consul_jsonify
{
    public class ConsulKv
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ConsulKv(string key, string value)
        {
            Key = key.Trim();
            Value = value?.Trim();
        }

        public override string ToString()
        {
            return string.Format("'{0}': '{1}'", Key, Value);
        }
    }
}
