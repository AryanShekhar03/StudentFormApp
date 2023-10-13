using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentRegisterApp.Models;

namespace StudentRegisterApp.Controllers
{
    public class StudentsController : Controller
    {
        private StudentFormDBEntities db = new StudentFormDBEntities();

        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.City1).Include(s => s.Country1).Include(s => s.State1);
            return View(students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name");
        //    ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Name");
        //    ViewBag.StateID = new SelectList(db.States, "StateID", "Name");
        //    return View();
        //}
        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.CountryList = new SelectList(db.Countries, "CountryID", "Name");
            ViewBag.StateList = new SelectList(db.States, "StateID", "Name");
            ViewBag.CityList = new SelectList(db.Cities, "CityID", "Name");
            return View();
        }


        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "StudentID,Name,Class,Roll,Address,Country,State,City,CountryID,StateID,CityID")] Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Students.Add(student);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", student.CityID);
        //    ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Name", student.CountryID);
        //    ViewBag.StateID = new SelectList(db.States, "StateID", "Name", student.StateID);
        //    return View(student);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentID,Name,Class,Roll,Address,CountryID,StateID,CityID")] Student student)
        {
            if (ModelState.IsValid)
            {
                // Fetch the selected Country, State, and City objects from the database.
                var country = db.Countries.Find(student.CountryID);
                var state = db.States.Find(student.StateID);
                var city = db.Cities.Find(student.CityID);

                // Set the Country, State, and City properties of the student object.
                student.Country = country.Name;
                student.State = state.Name;
                student.City = city.Name;

                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", student.CityID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Name", student.CountryID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "Name", student.StateID);
            return View(student);
        }



        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", student.CityID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Name", student.CountryID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "Name", student.StateID);
            return View(student);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "StudentID,Name,Class,Roll,Address,CountryID,StateID,CityID")] Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(student).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", student.CityID);
        //    ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Name", student.CountryID);
        //    ViewBag.StateID = new SelectList(db.States, "StateID", "Name", student.StateID);
        //    return View(student);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID,Name,Class,Roll,Address,CountryID,StateID,CityID")] Student student)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing student from the database
                Student existingStudent = db.Students.Find(student.StudentID);

                if (existingStudent != null)
                {
                    // Update the properties of the existing student
                    existingStudent.Name = student.Name;
                    existingStudent.Class = student.Class;
                    existingStudent.Roll = student.Roll;
                    existingStudent.Address = student.Address;
                    existingStudent.CountryID = student.CountryID;
                    existingStudent.StateID = student.StateID;
                    existingStudent.CityID = student.CityID;

                    // Update the student in the database
                    db.Entry(existingStudent).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }

            // If ModelState is not valid, re-populate the dropdowns and return to the edit view
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", student.CityID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Name", student.CountryID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "Name", student.StateID);
            return View(student);
        }
        public JsonResult GetStates(int countryId)
        {
            var states = db.States.Where(s => s.CountryID == countryId).Select(s => new { s.StateID, s.Name });
            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCities(int stateId)
        {
            var cities = db.Cities.Where(c => c.StateID == stateId).Select(c => new { c.CityID, c.Name });
            return Json(cities, JsonRequestBehavior.AllowGet);
        }




        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
