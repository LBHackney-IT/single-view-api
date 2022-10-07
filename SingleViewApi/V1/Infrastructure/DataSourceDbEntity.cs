using System.ComponentModel.DataAnnotations.Schema;

namespace SingleViewApi.V1.Infrastructure;
//TODO: rename table and add needed fields relating to the table columns.
// There's an example of this in the wiki https://github.com/LBHackney-IT/lbh-base-api/wiki/Database-contexts

//TODO: Pick the attributes for the required data source, delete the others as appropriate
// Postgres will use the "Table" and "Column" attributes

[Table("data_sources")]
public class DataSourceDbEntity
{
    [Column("id")] public int Id { get; set; }

    [Column("name")] public string Name { get; set; }
}
