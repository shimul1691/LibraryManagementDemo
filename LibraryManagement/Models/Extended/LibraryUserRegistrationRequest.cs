using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    [MetadataType(typeof(LibraryUserMetadata))]
    public partial class LibraryUserRegistrationRequest
    {
        public string ConfirmPassword { get; set; }
    }
    public class LibraryUserMetadata
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is Required")]
        public string MobileNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is Required")]
        public string HomeAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is Required")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country is Required")]
        public string Country { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is Required")]
        public string Uname { get; set; }

        
        [DataType(DataType.Password)]
        [Compare("Upassword", ErrorMessage = "confirm password does not match")]
        public string ConfirmPassword { get; set; }

    }
}
