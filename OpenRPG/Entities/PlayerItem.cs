using System.ComponentModel.DataAnnotations.Schema;

namespace OpenRPG.Entities
{
    public class PlayerItem
    {
        /// <summary>
        /// The unique ID of the item.
        /// </summary>
        public int PlayerItemId { get; set; }

        /// <summary>
        /// The player Id.
        /// </summary>
        [ForeignKey("Player")]
        public int PlayerId { get; set; }

        /// <summary>
        /// The player that the item belongs to.
        /// </summary>
        public virtual Player User { get; set; }
    }
}