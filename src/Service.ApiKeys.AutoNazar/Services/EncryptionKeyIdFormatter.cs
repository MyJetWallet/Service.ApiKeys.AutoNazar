namespace Service.ApiKeys.AutoNazar.Services
{
    public static class EncryptionKeyIdFormatter
    {
        public static string ToAutoNazar(string id)
        {
            return $"AutoNazar_{id}";
        }

        public static string FromAutoNazar(string id)
        {
            return id.Remove(0, "AutoNazar_".Length);
        }
    }
}
