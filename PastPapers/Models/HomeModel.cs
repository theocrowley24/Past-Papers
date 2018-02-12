using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using PastPapers.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PastPapers.Helpers;
using System.Globalization;
using Newtonsoft.Json;

namespace PastPapers.Models
{
    public class HomeModel
    {
        public HttpContext httpContext;

        //Add past paper attributes
        public string Subject { get; set; }
        public string Module { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public int Mark { get; set; }
        public int MaxMark { get; set; }
        public string Grade { get; set; }
        public string DateCompleted { get; set; }

        //Errors
        public bool AddPaperFailed { get; set; }
        public string AddPaperErrorMessage { get; set; }

        public int? NumberOfPapersToDisplay { get; set; }

        public string GraphDataJSON { get; set; }

        public HomeModel(HttpContext httpContext)
        {
            this.httpContext = httpContext;
            GetPapers();
        }

        public HomeModel()
        {
            GetPapers();
        }

        /// <summary>
        /// Gets the graph data neccessary for plotting graphs in the view
        /// </summary>
        public void GetGraphData()
        {

            List<PastPaper> papers = GetAllPapers();

            DateTime startDate = DateTime.Now.AddDays(-30);

            List<int> numberOfPapers = new List<int>();
            List<double> percentages = new List<double>();
            List<string> dates = new List<string>();

            foreach (DateTime day in DateHelper.EachDay(startDate, DateTime.Now))
            {
                var papersThisDay = papers.Where(a => a.dateCompleted == day.ToString("yyyy-MM-dd"));
                numberOfPapers.Add(papersThisDay.Count());
                dates.Add(day.ToString("yyyy-MM-dd"));

                double totalPercentage = 0;
                foreach(var paper in papersThisDay)
                {
                    totalPercentage += paper.percentage;
                }

                percentages.Add(totalPercentage / papersThisDay.Count());
            }

            GraphData graphData = new GraphData(dates, numberOfPapers, percentages);
            GraphDataJSON = JsonConvert.SerializeObject(graphData);

        }

        public void AddPastPaper()
        {
            double percentage = (Mark / MaxMark) * 100;

            try
            {
                DatabaseContext context = httpContext.RequestServices.GetService(typeof(DatabaseContext)) as DatabaseContext;
                MySqlConnection connection = context.GetConnection();
                connection.Open();
                string sql = "INSERT INTO papers (id, subject, month, year, mark, max_mark, percentage, grade, date_completed, user, module, user_id) VALUES (DEFAULT, @subject, @month, @year, @mark, @maxMark, @percentage, @grade, @dateCompleted, @user, @module, @userID)";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@subject", Subject);
                cmd.Parameters.AddWithValue("@month", Month);
                cmd.Parameters.AddWithValue("@year", Year);
                cmd.Parameters.AddWithValue("@mark", Mark);
                cmd.Parameters.AddWithValue("@maxMark", MaxMark);
                cmd.Parameters.AddWithValue("@percentage", percentage);
                cmd.Parameters.AddWithValue("@grade", Grade);
                cmd.Parameters.AddWithValue("@dateCompleted", DateCompleted);
                cmd.Parameters.AddWithValue("@user", httpContext.Session.GetString("username"));
                cmd.Parameters.AddWithValue("@module", Module);
                cmd.Parameters.AddWithValue("@userID", context.GetUserID(httpContext.Session.GetString("username")));
                cmd.ExecuteNonQuery();
                connection.Close();

            } catch (Exception ex)
            {
                AddPaperFailed = true;
                AddPaperErrorMessage = "Error when retrieving from server";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public List<PastPaper> GetPapers()
        {
            List<PastPaper> papers = new List<PastPaper>();

            try
            {
                DatabaseContext context = httpContext.RequestServices.GetService(typeof(DatabaseContext)) as DatabaseContext;
                MySqlConnection connection = context.GetConnection();
                connection.Open();
                string sql = "SELECT * FROM papers WHERE user = @username";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", httpContext.Session.GetString("username"));
                MySqlDataReader reader = cmd.ExecuteReader();

                int counter = 0;

                NumberOfPapersToDisplay = NumberOfPapersToDisplay ?? 5;

                while (reader.Read() && counter < NumberOfPapersToDisplay)
                {
                    counter++;
                    papers.Add(new PastPaper(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetFloat(6), reader.GetString(7), reader.GetString(8), reader.GetString(10)));
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return papers;
        }

        public List<PastPaper> GetAllPapers()
        {
            List<PastPaper> papers = new List<PastPaper>();

            try
            {
                DatabaseContext context = httpContext.RequestServices.GetService(typeof(DatabaseContext)) as DatabaseContext;
                MySqlConnection connection = context.GetConnection();
                connection.Open();
                string sql = "SELECT * FROM papers WHERE user = @username";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", httpContext.Session.GetString("username"));
                MySqlDataReader reader = cmd.ExecuteReader();

                NumberOfPapersToDisplay = NumberOfPapersToDisplay ?? 5;

                while (reader.Read())
                {
                    papers.Add(new PastPaper(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetFloat(6), reader.GetString(7), reader.GetString(8), reader.GetString(10)));
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return papers;
        }
    }
}
