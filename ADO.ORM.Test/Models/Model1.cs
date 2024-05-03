using ADO.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.ORM.Test.Models; 
public class Model1 {
    [PrimaryKey]
    public int Model1Id { get; set; }
    public string Nome { get; set; }
    public string Sobrenome { get; set; }
}
