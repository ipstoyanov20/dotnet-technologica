using System.ComponentModel.DataAnnotations.Schema;
using TL.AspNet.Security.Abstractions.User;
using TL.Common.Models.Attributes;

namespace TL.DataAccess.Models
{
    public partial class Operator : IPasswordUser<decimal>
    {
        [NotMapped]
        [EntityPropertyName(nameof(Email))]
        public string Username { get => this.Email; }

        [NotMapped]
        [EntityPropertyName(nameof(Id))]
        public decimal UserIdentifier => this.Id;
    }
}
