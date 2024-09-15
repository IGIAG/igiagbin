using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace igiagbin.Models
{
	public class PasteModel
	{
		[BindNever]
		public string? Id { get; set; } = "PasteModel|";

        [BindNever]
        public DateTime UploadedOn { get; set; } = DateTime.Now;

		[BindRequired]
		[StringLength(10000, ErrorMessage = "Paste must be between 1 and 10000 characters", MinimumLength = 1)]
		public string Contents { get; set; } = "No contents";


	}
}
