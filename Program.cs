using System.Collections;
using System.Data;
using Dapper;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
namespace DataAccess
{
    class Program
    {
        //! Utilizando Dapper
        static void Main()
        {
            Console.Clear();
            const string connectionString = "Server=NTBGFINF13\\SQLEXPRESS;Database=balta;Trusted_Connection=True;TrustServerCertificate=true;";
            //Guid del = new Guid("04cbf285-798c-4521-a25e-d5dda5147e76");
            using (var conn = new SqlConnection(connectionString))
            {
                //UpdateCategory(conn);
                //CreateManyCategorys(conn);
                //ListCategorias(conn);
                //DeleteCategory(conn, del);
                //ExeceuteProcedure(conn);
                //ExeceuteReadProcedure(conn);
                //ExecuteScalar(conn);
                //ReadView(conn);
                //OneToOne(conn);
                // OneToMany(conn);
                //QueryMultiple(conn);
                //SelectIn(conn);
                //Like(conn, ".Net");
                Transactions(conn);
            }

        }

        static void ListCategorias(SqlConnection conn)
        {
            var categories = conn.Query<Category>("SELECT Id, Title from Category");

            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void CreateCategory(SqlConnection conn)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços em nuvem";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insert = @"INSERT INTO 
                            Category 
                        VALUES(
                            @Id, 
                            @Title, 
                            @Url, 
                            @Summary,
                            @Order, 
                            @Description, 
                            @Featured
                            )";

            var rows = conn.Execute(insert, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void UpdateCategory(SqlConnection conn)
        {
            var updateQuery = "UPDATE Category SET Title=@title WHERE Id=@id";

            var rows = conn.Execute(updateQuery, new
            {
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "Frontend 2023"
            });

            string registro = rows > 1 ? "registros atualizados" : "registro atulizado";

            Console.WriteLine($"{rows} {registro}");
        }

        static void DeleteCategory(SqlConnection conn, Guid IdDel)
        {
            var deleteQuery = "Delete from Category where Id=@id";

            var rows = conn.Execute(deleteQuery, new
            {
                id = IdDel
            });

            string registro = rows > 1 ? "registros atualziados" : "registro atualizado";

            Console.WriteLine($"{rows} {registro}");
        }

        static void CreateManyCategorys(SqlConnection conn)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços em nuvem";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "NextJS";
            category2.Url = "javascript";
            category2.Description = "Categoria destinada a serviços front-end";
            category2.Order = 9;
            category2.Summary = "Next";
            category2.Featured = true;
            var insert = @"INSERT INTO 
                            Category 
                        VALUES(
                            @Id, 
                            @Title, 
                            @Url, 
                            @Summary,
                            @Order, 
                            @Description, 
                            @Featured
                            )";

            var rows = conn.Execute(insert, new[]{
                new  {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
                },
                new {
                    category2.Id,
                    category2.Title,
                    category2.Url,
                    category2.Summary,
                    category2.Order,
                    category2.Description,
                    category2.Featured
                }
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void ExeceuteProcedure(SqlConnection conn)
        {
            var procedure = "[spDeleteStudent]";

            var param = new { StudentId = "65084ca1-f5c9-4e1a-909f-80d3ee0167fa" };

            var rows = conn.Execute(
                procedure,
                param,
                commandType: CommandType.StoredProcedure);

            string registro = rows > 1 ? "registros atualziados" : "registro atualizado";

            Console.WriteLine($"{rows} {registro}");
        }

        static void ExeceuteReadProcedure(SqlConnection conn)
        {
            var procedure = "[spGetCoursesByCategory]";

            var param = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };

            var rows = conn.Query<Category>(
                procedure,
                param,
                commandType: CommandType.StoredProcedure);

            foreach (var item in rows)
            {
                Console.WriteLine(item.Id);
            }
        }

        static void ExecuteScalar(SqlConnection conn)
        {
            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços em nuvem";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insert = @"
                        INSERT INTO 
                            Category 
                        OUTPUT inserted.[Id]
                        VALUES(
                            NEWID(), 
                            @Title, 
                            @Url, 
                            @Summary,
                            @Order, 
                            @Description, 
                            @Featured)
                            ";

            var id = conn.ExecuteScalar<Guid>(insert, new
            {
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            Console.WriteLine($"O id lançado para esta nova categoria é: {id}");
        }

        static void ReadView(SqlConnection conn)
        {
            var sql = "SELECT * FROM vwCourses";

            var courses = conn.Query(sql);

            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void OneToOne(SqlConnection conn)
        {
            var sql = @"
                SELECT 
                *
                FROM CareerItem
                INNER JOIN Course
                ON CareerItem.CourseId = Course.Id
            ";

            var items = conn.Query<CareerItem, Course, CareerItem>(sql,
            (careerItem, course) =>
            {
                careerItem.Course = course;
                return careerItem;
            },
            splitOn: "Id"
            );

            foreach (var item in items)
            {
                Console.WriteLine(item.Course.Title);
            }
        }

        static void OneToMany(SqlConnection conn)
        {
            var sql = @"
                SELECT 
                    Career.Id,
                    Career.Title,
                    CareerItem.CareerId,
                    CareerItem.Title
                FROM 
                    Career 
                INNER JOIN 
                    CareerItem ON CareerItem.CareerId = Career.Id
                ORDER BY
                    Career.Title";

            var careers = new List<Career>();
            var items = conn.Query<Career, CareerItem, Career>(
                sql,
                (career, item) =>
                {
                    var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                    if (car == null)
                    {
                        car = career;
                        car.Items.Add(item);
                        careers.Add(car);
                    }
                    else
                    {
                        car.Items.Add(item);
                    }

                    return career;
                }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                Console.WriteLine($"{career.Title}");
                foreach (var item in career.Items)
                {
                    Console.WriteLine($" - {item.Title}");
                }
            }
        }

        static void QueryMultiple(SqlConnection conn)
        {
            var query = "SELECT * FROM Category; Select * from Course";

            using (var multi = conn.QueryMultiple(query))
            {
                var categories = multi.Read<Category>();
                var course = multi.Read<Course>();

                foreach (var item in categories)
                {
                    Console.WriteLine($"Categoria - {item.Title}");
                }


                foreach (var item in course)
                {
                    Console.WriteLine($"Curso - {item.Title}");
                }
            }
        }

        static void SelectIn(SqlConnection conn)
        {
            var query = @"select * from Career where Id IN @id";

            var carreiras = conn.Query<Career>(query, new
            {
                id = new[] {"01ae8a85-b4e8-4194-a0f1-1c6190af54cb",
                "e6730d1c-6870-4df3-ae68-438624e04c72"}
            });

            foreach (var item in carreiras)
            {
                Console.WriteLine(item.Title);
            }
        }

        static void Like(SqlConnection conn, string termo)
        {
            var query = @"select * from Course where Title LIKE @exp";

            var carreiras = conn.Query<Course>(query, new
            {
                exp = $"%{termo}%"
            });

            foreach (var item in carreiras)
            {
                Console.WriteLine(item.Title);
            }
        }

        static void Transactions(SqlConnection conn)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Nova categoria";
            category.Url = "Nova";
            category.Description = "Categoria destinada a serviços em nuvem";
            category.Order = 8;
            category.Summary = "Nova cateria nova";
            category.Featured = false;

            var insert = @"INSERT INTO 
                            Category 
                        VALUES(
                            @Id, 
                            @Title, 
                            @Url, 
                            @Summary,
                            @Order, 
                            @Description, 
                            @Featured
                            )";
            conn.Open();
            using (var transaction = conn.BeginTransaction())
            {
                var rows = conn.Execute(insert, new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                }, transaction);

                transaction.Commit();
                //transaction.Rollback();
                
                Console.WriteLine($"{rows} linhas inseridas");
            }
        }
        //! Utilizando Data SQLClient
        // static void Main()
        // {
        //     const string connectionString = "Server=NTBGFINF13\\SQLEXPRESS;Database=balta;Trusted_Connection=True;TrustServerCertificate=true;";

        //     var connection = new SqlConnection();

        //     using (var conn = new SqlConnection(connectionString))
        //     {
        //         Console.WriteLine("Conectado...");
        //         conn.Open();


        //         using (var command = new SqlCommand())
        //         {
        //             command.Connection = conn;
        //             command.CommandType = System.Data.CommandType.Text;
        //             command.CommandText = "SELECT Id, Title from Category";

        //             var reader = command.ExecuteReader();
        //             while (reader.Read())
        //             {
        //                 Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
        //             }
        //         }
        //     }

        // connection.Dispose(); //Destroi a conexão
        // }
    }
}