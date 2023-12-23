using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCosmos.Models;

namespace MvcCosmos.Controllers
{
    public class InstructionsController : Controller
    {
        private readonly IDocumentDBRepository<Instruction> Repository;
        public InstructionsController(IDocumentDBRepository<Instruction> Repository)
        {
            this.Repository = Repository;
        }

        // GET: Instructions
        public async Task<IActionResult> Index()
        {
            var instructions = await Repository.GetItemsAsync(item => item.Id != null).ConfigureAwait(true);
            return View(instructions.OrderBy(item => item.Step));
        }

        // GET: Instructions/Details/Id
        public async Task<IActionResult> Details(string id)
        {
            Instruction instruction = await Repository.GetItemAsync(id).ConfigureAwait(true);
            return PartialView("_Details", instruction);
        }

        // GET: Instructions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Step,Type,Value")] Instruction instruction)
        {
            if (ModelState.IsValid)
            {
                await Repository.CreateItemAsync(instruction).ConfigureAwait(true);
                return RedirectToAction(nameof(Index));
            }
            return View(instruction);
        }

        // GET: Instructions/Edit/Id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Instruction instruction = await Repository.GetItemAsync(id).ConfigureAwait(true);
            if (instruction == null)
            {
                return NotFound();
            }
            return View(instruction);
        }

        // POST: Instructions/Edit/Id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Step,Type,Value")] Instruction instruction)
        {
            if (id != instruction.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await Repository.UpdateItemAsync(instruction.Id, instruction).ConfigureAwait(true);
                return RedirectToAction(nameof(Index));
            }
            return View(instruction);
        }

        // GET: Instructions/Delete/Id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Instruction instruction = await Repository.GetItemAsync(id).ConfigureAwait(true);
            if (instruction == null)
            {
                return NotFound();
            }

            return View(instruction);
        }

        // POST: Instructions/Delete/Id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("Id")] string id)
        {
            await Repository.DeleteItemAsync(id).ConfigureAwait(true);
            return RedirectToAction(nameof(Index));
        }

        // GET: Instructions/Upload
        [ActionName("Upload")]
        public async Task<IActionResult> UploadToCosmosDB()
        {
            // TODO: show some loading animation

            //string textFile = "~/uploads/operations.txt";
            Uri textFileUri = new Uri("https://scorpiusworks.com/Test/operations.txt");
            var textLines = Instruction.ReadDataFromText(textFileUri);
            var instructions = Instruction.CreateInstructionsListFromText(textLines);

            var allTasks = new List<Task>();
            foreach (var instruction in instructions)
            {
                allTasks.Add(Repository.CreateItemAsync(instruction));
            }
            await Task.WhenAll(allTasks).ConfigureAwait(true);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Instructions/Cleanup
        [ActionName("Cleanup")]
        public async Task<IActionResult> CleanupCosmosDB()
        {
            var instructions = await Repository.GetItemsAsync(a => a.Id != null).ConfigureAwait(true);
            var allTasks = new List<Task>();
            foreach (var instruction in instructions)
            {
                allTasks.Add(Repository.DeleteItemAsync(instruction.Id));
            }
            await Task.WhenAll(allTasks).ConfigureAwait(true);

            return RedirectToAction(nameof(Index));
        }

        // GET: Instructions/GetTravel
        [ActionName("GetTravel")]
        public async Task<IActionResult> GetTravel()
        {
            Vehicle vehicle = new Vehicle();
            var markerA = (await Repository.GetItemsAsync(a => a.Type == "marker" && a.Value == "A").ConfigureAwait(true)).FirstOrDefault().Step;
            var markerC = (await Repository.GetItemsAsync(c => c.Type == "marker" && c.Value == "C").ConfigureAwait(true)).FirstOrDefault().Step;

            var travelList = await Repository.GetItemsAsync(t => t.Step > markerA && t.Step < markerC).ConfigureAwait(true);
            foreach (var instruction in travelList)
            {
                vehicle.Act(instruction);
            }

            ViewData["distanceTraveled"] = vehicle.DistanceTravelled;
            ViewData["distanceBetweenMarkers"] = Vehicle.GetDistanceFromStartPoint(vehicle.CurrentLocation).ToString("F", CultureInfo.InvariantCulture);
            return PartialView("_TripStatus");
        }
    }
}
