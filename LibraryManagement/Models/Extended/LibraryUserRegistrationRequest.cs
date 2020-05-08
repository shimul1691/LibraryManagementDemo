using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;


namespace LibraryManagement.Models
{
    [ModelMetadataType(typeof(LibraryUserMetadata))]
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

        [CompareOtherPropertyEmptyOrNull("PassportNo", ErrorMessage ="You must provide either valid national id number or passport number")]
        public string Nidno { get; set; }

        [CompareOtherPropertyEmptyOrNull("Nidno", ErrorMessage = "You must provide either valid national id number or passport number")]
        public string PassportNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is Required")]
        public string MobileNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is Required")]
        public string HomeAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is Required")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country is Required")]
        public string Country { get; set; }

        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is Required")]
        public string Uname { get; set; }

        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password field is required")]
        [MinLength(6, ErrorMessage = "password must be minimum 6 characters and maximum 8 characters long")]
        [MaxLength(8, ErrorMessage = "password must be minimum 6 characters and maximum 8 characters long")]
        [RegularExpression("([a-zA-Z]{1,})([@$!%*#?&]{1,})([0-9]{1,})", ErrorMessage = "password must be minimum 6 characters and maximum 8 characters long and contain atleast one special character one indian arabian numeric digit and one upper case an one lower case from roman character set")]
        public string Upassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Upassword", ErrorMessage = "confirm password does not match")]
        [Required]
        public string ConfirmPassword { get; set; }

    }
}
