//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Change.Models
{
    using System;
    using System.Collections.Generic;
    //
    using System.ComponentModel.DataAnnotations;
    public partial class Photo
    {
        public int PhotoId { get; set; }
        [Display(Name = "商品")]
        public Nullable<int> ProductId { get; set; }
        [Display(Name = "图片地址")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string PhotoUrl { get; set; }

        public virtual Product Product { get; set; }
    }
}
