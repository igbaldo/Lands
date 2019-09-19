using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Lands.Domain;

namespace Lands.Backend.Models
{
    public class TeamView : Team
    {
        [Display(Name = "Image")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}