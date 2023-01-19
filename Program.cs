using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace AABD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String conSir =
        "Data Source=.;Initial Catalog=BPersonal;Integrated Security=True;" +
        "Initial Catalog=\"BPersonal\";" +
        "Integrated Security=SSPI;" +
        "MultipleActiveResultSets=true;";
            var conOb = new SqlConnection(conSir);


            String conSir2 =
                "Data Source=Data Source=.;Initial Catalog=BPersonal;Integrated Security=True;" +
                "Initial Catalog=\"BStudenti\";" +
                "Integrated Security=SSPI;" +
                "MultipleActiveResultSets=true;";
            var conOb2 = new SqlConnection(conSir2);



            Program.Problema1(conOb);
            //Program.Problema2(conOb);
            //Program.Problema5(conOb);
            //Program.Problema11a(conOb);
            //Program.Problema11b(conOb);
            //Program.Problema11c(conOb);
        }

        static void Problema1(SqlConnection conOb)
        {

            string query = "select * from Salariati Where Profesia='Vopsitor' AND Salariu>1500 ORDER BY Salariu DESC, nume ASC; ";

            SqlCommand command = new SqlCommand(query, conOb);


            conOb.Open();


            SqlDataReader sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
            {
                string rec = "";
                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    rec += sqlDataReader[i] + " | ";
                }
                Console.WriteLine(rec);
            };
            conOb.Close();

            Console.ReadLine();
        }

        static void Problema2(SqlConnection conOb)
        {
          
            string query = @"SELECT e.Nume, e.Prenume , e.Sex , e.Salariu , e.Profesia , e.DataAngajarii , e.DataNasterii , e.AreCopii , d.Suma  as suma_retinere
        FROM Salariati e
        JOIN Retineri d
        ON e.Marca = d.Marca
        ORDER BY e.Nume, e.Prenume; ";

            SqlCommand command = new SqlCommand(query, conOb);

            conOb.Open();


            SqlDataReader sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
            {
                string rec = "";
                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    rec += sqlDataReader[i] + " | ";
                }
                Console.WriteLine(rec);
            };
            conOb.Close();
            Console.WriteLine("f");
            Console.ReadLine();

        }




        public static void Problema5(SqlConnection conOb)
        {

            string retineriSalariati = "select Salariati.Marca, Salariati.Nume, Salariati.Prenume, Salariati.Salariu, Retineri.TipRetinere, " +
                "Retineri.Suma Suma_datorata from Salariati, Retineri where Salariati.Marca = Retineri.Marca";

            try
            {
                conOb.Open();

                Console.WriteLine("\nSituatie retineri salariale\n ");
                var r1 = SalariatiRetineri(retineriSalariati, conOb);

                Console.WriteLine("Marca\t\tNume\t\tPrenume\t\tSalariu\t\tTipRetinere\t\tSuma_datorata\n");

                while (r1.Read())
                {
                    Console.WriteLine("{0}\t\t{1}\t\t{2}\t\t{3}\t\t{4}\t\t{5}", r1["Marca"], r1["Nume"], r1["Prenume"],
                        r1["Salariu"], r1["TipRetinere"], r1["Suma_datorata"]);
                }
                Console.Read();
            }
            catch (SqlException exc)
            {

                Console.WriteLine(" Eroare de conexiune: " + exc.Message);
                Console.Read();
            }
            finally
            {

                conOb.Close();
            }
        }

        public static SqlDataReader SalariatiRetineri (string nr, SqlConnection conOb)
        {
            var r1 = new SqlCommand(nr, conOb);
            return (SqlDataReader)r1.ExecuteReader();
        }








        public static void Problema11a(SqlConnection conOb)
        {
            /////a)Sa se scrie o aplicatie consola care sa precizeze numarul de luni lucrate de angajati. Solutie cu UDF

            string query = @"CREATE FUNCTION LuniLucrate 
        (
            @DateA DATETIME,
            @DateB DATETIME
        )
        RETURNS INT
        AS
        BEGIN
            DECLARE @Result INT

            DECLARE @DateX DATETIME
            DECLARE @DateY DATETIME

            IF(@DateA < @DateB)
            BEGIN
                SET @DateX = @DateA
                SET @DateY = @DateB
            END
            ELSE
            BEGIN
                SET @DateX = @DateB
                SET @DateY = @DateA
            END

            SET @Result = (
                            SELECT 
                            CASE 
                                WHEN DATEPART(DAY, @DateX) > DATEPART(DAY, @DateY)
                                THEN DATEDIFF(MONTH, @DateX, @DateY) - 1
                                ELSE DATEDIFF(MONTH, @DateX, @DateY)
                            END
                            )

            RETURN @Result
        END";

            SqlCommand command = new SqlCommand(query, conOb);


            conOb.Open();
            SqlDataReader sqlDataReader;


            query = "Select Nume, Prenume , Sex , Salariu , Profesia as result from Salariati";
            command = new SqlCommand(query, conOb);


            sqlDataReader = command.ExecuteReader();

            Console.WriteLine("Nume, Prenume | Sex | Salariu | Profesie | TotalLuniLucrate");
            while (sqlDataReader.Read())
            {
                string rec = "";
                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    rec += sqlDataReader[i] + " | ";
                }
                Console.WriteLine(rec);
            };
            conOb.Close();
            Console.ReadLine();
        }





       public static void Problema11b(SqlConnection conOb)
        {
            /////Sa se scrie o procedura stocata care sa primeasca numarul total de barbati si femei angajate.
            ///
            string query = @"CREATE PROCEDURE CountBarbatiFemei
                      AS
                      select  COUNT(*)as tot, 
                      COUNT(case when Sex = 'M' then 1 end) as CountBarbati,
                      COUNT(case when Sex = 'F' then 1 end) as CountFemei
                      from Salariati";

            SqlCommand command = new SqlCommand(query, conOb);


            conOb.Open();
            SqlDataReader sqlDataReader;


            query = @"EXEC CountBarbatiFemei";

            command = new SqlCommand(query, conOb);


            sqlDataReader = command.ExecuteReader();

            Console.WriteLine("Total | Barbati | Femei");
            while (sqlDataReader.Read())
            {
                string rec = "";
                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    rec += sqlDataReader[i] + " | ";
                }
                Console.WriteLine(rec);
            };
            conOb.Close();
            Console.ReadLine();
        }



        //Sa se creeze o tranzactie pentru a insera datele unui salariat
        public static void Problema11c(SqlConnection conOb)
        {


            SqlCommand trancaztie = new SqlCommand("INSERT INTO Salariati(Marca, Nume, Prenume, Sex, Salariu, Profesia, DataAngajarii, DataNasterii, AreCopii, NrCopii) " +
              " Values(1238, 'Mihai', 'Vasile', 'M', 2500, 'Vopsitor', '2005 - 07 - 25', '1980-08-23', 0, 0)", conOb);
try
            {
                conOb.Open();
                
                SqlTransaction tran = conOb.BeginTransaction();
                

                trancaztie.Transaction = tran;
            
                trancaztie.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conOb.Close();
            }
        }
    }
       
       
    
}
