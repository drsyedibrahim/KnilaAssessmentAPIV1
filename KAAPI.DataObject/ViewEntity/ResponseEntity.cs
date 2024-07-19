using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAAPI.DataObject.ViewEntity
{
    public class ResponseEntity<T>
    {
        public T? Result { get; set; }
        public bool IsSuccess { get; set; }
        public string? ResponseMessage { get; set; }
        public int StatusCode { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? StatusMessage { get; set; }
        public int? TotalRecords { get; set; }
        public int? TotalPages { get; set; }
    }
}
