using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Domain.Models.EncryptionKeys
{
    public class EncryptionKey
    {
        public string Id { get; set; }

        public string EncryptionKeyValue { get; set; }

        public string CheckWord { get; set; }
        public string ServiceName { get; set; }
    }
}
