﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Instructors
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, SlackHandle, CohortId, Specialty FROM Instructor";
              

                    var reader = cmd.ExecuteReader();
                    var instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        var instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))

                        };
                        instructors.Add(instructor);
                    }
                    reader.Close();
                    return View(instructors);
                }
            }
        }

        // GET: Instructors/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle, CohortId, Specialty
                                    FROM Instructor
                                    WHERE Id =@id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));



                    var reader = cmd.ExecuteReader();
                    Instructor instructor = null;

                    if (reader.Read())
                    {
                      instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))

                        };
                        
                    }
                    reader.Close();
                    return View(instructor);
                }
            }
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            var cohortOptions = GetCohortOptions();
            var viewModel = new InstructorEditViewModel()
            {
                CohortOptions = cohortOptions
            };
            return View(viewModel);
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, Specialty, CohortId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstName, @lastName, @slackHandle, @specialty, @cohortId)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@specialty", instructor.Specialty));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", instructor.CohortId));

                        var id = (int)cmd.ExecuteScalar();
                        instructor.Id = id;

                        return RedirectToAction(nameof(Index));
                    }
                }
;
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Instructors/Edit/5
        public ActionResult Edit(int id)
        {
            var instructor = GetInstructorById(id);
            var cohortOptions = GetCohortOptions();
            var viewModel = new InstructorEditViewModel()
            {
                InstructorId = instructor.Id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                CohortId = instructor.CohortId,
                SlackHandle = instructor.SlackHandle,
                Specialty = instructor.Specialty,
                CohortOptions = cohortOptions
            };
            return View(viewModel);
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" UPDATE Instructor
                                           SET FirstName = @FirstName, 
                                           LastName = @LastName,
                                           SlackHandle = @SlackHandle,
                                           CohortId = @CohortId,
                                           Specialty = @Specialty
                                           WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@FirstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@SlackHandle", instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@CohortId", instructor.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@Specialty", instructor.Specialty));
                        cmd.Parameters.Add(new SqlParameter("@id", instructor.Id));

                        var rowsaffected = cmd.ExecuteNonQuery();

                        if (rowsaffected < 1)
                        {
                            return NotFound();

                        }
                    }
                }

                    return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Instructors/Delete/5
        public ActionResult Delete(int id)
        {
            var instructor = GetInstructorById(id);
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Instructor WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        private Instructor GetInstructorById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, CohortId, Specialty, SlackHandle FROM Instructor WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                        };

                    }
                    reader.Close();
                    return instructor;
                }
            }
        }


        private List<SelectListItem> GetCohortOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohort";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("Name")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }

    }
}