using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SingleViewApi.V1.Infrastructure;
//TODO: rename table and add needed fields relating to the table columns.
// There's an example of this in the wiki https://github.com/LBHackney-IT/lbh-base-api/wiki/Database-contexts

//TODO: Pick the attributes for the required data source, delete the others as appropriate
// Postgres will use the "Table" and "Column" attributes

[Table("customers")]
public class CustomerDbEntity
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("first_name")] public string FirstName { get; set; }

    [Column("last_name")] public string LastName { get; set; }

    [Column("date_of_birth")] public DateTime DateOfBirth { get; set; }

    [Column("ni_number")] public string NiNumber { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("updated_at")] public DateTime UpdatedAt { get; set; }

    public List<CustomerDataSourceDbEntity> DataSources { get; set; }
}
