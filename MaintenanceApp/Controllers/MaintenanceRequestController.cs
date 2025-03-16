using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;
using MaintenanceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;

namespace MaintenanceApp.Controllers
{
    public class MaintenanceRequestController : Controller
    {
        private readonly MaintenanceDbContext _context;

        public MaintenanceRequestController(MaintenanceDbContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MaintenanceRequest/Maintainer/AcknowledgeSubmitEdit")]
        public async Task<IActionResult> AcknowledgeSubmitEdit(RequestMaintenanceModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    List<PhraseModel> phrases = new List<PhraseModel>();
                    List<PhraseModel> RequestFor = new List<PhraseModel>();
                    List<PhraseModel> RequestStatus = new List<PhraseModel>();

                    var existingRecord = await _context.requestMaintenanceModel.FindAsync(model.Id);

                    if (existingRecord == null)
                    {
                        // Handle the case where the record doesn't exist
                        return NotFound("The record you are trying to edit does not exist.");
                    }


                    existingRecord.FormStatus = "Acknowledged";

                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionTakenBy));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionTakenDate));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionApprovedBy));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionApprovedDate));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionAcknowledgedBy));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionAcknowledgedDate));

                    // 3. Save changes to the database (both deletions and insertions)
                    await _context.SaveChangesAsync();

                    // 4. Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // If something fails, roll back the transaction
                    await transaction.RollbackAsync();
                    // Handle the exception or show an error message
                    return View("Error");
                }
            }
            return RedirectToAction("Index", new { tab = "Acknowledged" });

        }

        
        [HttpGet]
        [Route("MaintenanceRequest/Acknowledge/Edit/{id}")]
        public IActionResult AcknowledgeEdit(int id)
        {
            List<PhraseModel> phrases = new List<PhraseModel>();
            List<PhraseModel> RequestFor = new List<PhraseModel>();
            List<PhraseModel> RequestStatus = new List<PhraseModel>();

            var item = _context.requestMaintenanceModel.Find(id);

            if (item.FormStatus == "Acknowledged")
            {
                TempData["ErrorMessage"] = "You can no longer edit this request.";
                return RedirectToAction("Index");
            }

            var Phrasemodel = _context.phraseModel.Where(p => p.RequestMaintenanceID == id).ToList();
            item.phraseModel = Phrasemodel;
            var CheckboxesList = PhraseModel.PredefinedPhrases();
            foreach (var checkbox in CheckboxesList)
            {
                foreach (var option in item.phraseModel)
                {
                    if (checkbox.Phrase == option.Phrase && checkbox.Category == option.Category)
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            item.phraseModel = CheckboxesList;

            if (item == null)
            {
                return NotFound();
            }

            foreach (var option in item.phraseModel)
            {
                if (option.Category == "Request Status" && option.IsChecked)
                    item.RequestStatus = option.Phrase;
                if (option.Category == "Machine" && option.IsChecked)
                    item.MachineState = option.Phrase;
                if (option.Category == "Machine Status 2" && option.IsChecked)
                    item.MachineState2 = option.Phrase;
                if (option.Category == "Status of Request" && option.IsChecked)
                    item.StatufOfRequest = option.Phrase;

            }

            return View("Acknowledge/Edit", item);
        }

       
        [HttpPost]
        [Route("MaintenanceRequest/Delete")]
        public IActionResult DeleteAction([FromForm] int id, [FromForm] string FormStatus)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var request = _context.requestMaintenanceModel.FirstOrDefault(r => r.Id == id);

                if (request == null)
                {
                    return NotFound(new { message = "Request not found" });
                }

                // Perform different operations based on FormStatus
                switch (FormStatus)
                {
                    case "New":
                        _context.phraseModel.RemoveRange(_context.phraseModel.Where(p => p.RequestMaintenanceID == id));
                        _context.requestMaintenanceModel.Remove(request);
                        break;

                    case "Pending":
                     
                        _context.phraseModel.RemoveRange(_context.phraseModel.Where(p => p.RequestMaintenanceID == id && p.Category == "Review of Problems"));
                        request.ReviewRemarks = null;
                        request.FormStatus = "New";
                        break;

                    case "Completed":
                        _context.phraseModel.RemoveRange(_context.phraseModel.Where(p => p.RequestMaintenanceID == id && p.Category == "Machine Status 2" || p.Category == "Status of Request"));
                        request.DescriptionAction = null;
                        request.MachineState2 = null;
                        request.MachineRunning = null;
                        request.RequestOpen = null;
                        request.StatufOfRequest = null;
                        request.FormStatus = "Pending";
                        break;
                    case "Acknowledged":
                        request.ActionTakenBy = null;
                        request.ActionTakenDate = null;
                        request.ActionAcknowledgedBy = null;
                        request.ActionAcknowledgedDate = null;
                        request.FormStatus = "Completed";
                        break;
                    default:
                        return BadRequest(new { message = "Invalid FormStatus" });
                }

                _context.SaveChanges();
                transaction.Commit();

                return Ok(new { message = "Operation completed successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, new { message = "An error occurred during deletion.", error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MaintenanceRequest/Maintainer/MaintainerSubmitEdit2")]
        public async Task<IActionResult> MaintainerSubmitEdit2(RequestMaintenanceModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    List<PhraseModel> phrases = new List<PhraseModel>();
                    List<PhraseModel> RequestFor = new List<PhraseModel>();
                    List<PhraseModel> RequestStatus = new List<PhraseModel>();
                    model.FormStatus = "Completed";

                    var existingRecord = await _context.requestMaintenanceModel.FindAsync(model.Id);

                    if (existingRecord == null)
                    {
                        // Handle the case where the record doesn't exist
                        return NotFound("The record you are trying to edit does not exist.");
                    }

                    existingRecord.DescriptionAction = model.DescriptionAction;
                    existingRecord.RequestOpen = model.RequestOpen;
                    existingRecord.MachineRunning = model.MachineRunning;
                    existingRecord.FormStatus = model.FormStatus;

                    foreach (var property in typeof(RequestMaintenanceModel).GetProperties())
                    {
                        ModelState.Remove(property.Name);
                    }

                    var requiredFields = new Dictionary<string, object>
                        {
                            { nameof(model.DescriptionAction), model.DescriptionAction },
                            { nameof(model.MachineRunning), model.MachineRunning },
                            { nameof(model.RequestOpen), model.RequestOpen }
                        };

                    foreach (var field in requiredFields)
                    {
                        if (field.Value == null)
                        {
                            ModelState.AddModelError(field.Key, $"{field.Key} is required.");
                        }
                    }


                    var fieldsToValidate = new Dictionary<string, string>
                            {
                                { nameof(model.MachineState2), model.MachineState2 },
                                { nameof(model.StatufOfRequest), model.StatufOfRequest }
                            };

                    foreach (var field in fieldsToValidate)
                    {
                        if (string.IsNullOrWhiteSpace(field.Value))
                        {
                            ModelState.AddModelError(field.Key, $"{field.Key.Replace("model.", "")} is required.");
                        }
                    }

                    var previousPhrases = _context.phraseModel.Where(p => p.RequestMaintenanceID == model.Id && (p.Category == "Machine Status 2" || p.Category == "Status of Request")).ToList();

                    if (previousPhrases != null && previousPhrases.Any())
                    {
                        _context.phraseModel.RemoveRange(previousPhrases);
                    }

                    phrases.Add(new PhraseModel { Phrase = model.MachineState2, Category = "Machine Status 2", IsChecked = true });
                    phrases.Add(new PhraseModel { Phrase = model.StatufOfRequest, Category = "Status of Request", IsChecked = true });

                    // 2. Add the new PhraseModel entries
                    foreach (var selected in phrases)
                    {
                        selected.RequestMaintenanceID = model.Id;
                        _context.phraseModel.Add(selected);
                    }

                    // 3. Save changes to the database (both deletions and insertions)
                    await _context.SaveChangesAsync();

                    // 4. Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // If something fails, roll back the transaction
                    await transaction.RollbackAsync();
                    // Handle the exception or show an error message
                    return View("Error");
                }
            }
            TempData["SuccessMessage"] = "Record saved successfully!";
            return RedirectToAction("Index", new { tab = "Completed" });

        }

        
        [HttpGet]
        [Route("MaintenanceRequest/Maintainer/Edit2/{id}")]
        public IActionResult MaintainerEdit2(int id)
        {


            List<PhraseModel> phrases = new List<PhraseModel>();
            List<PhraseModel> RequestFor = new List<PhraseModel>();
            List<PhraseModel> RequestStatus = new List<PhraseModel>();

            var item = _context.requestMaintenanceModel.Find(id);
            if (item.FormStatus == "Acknowledged")
            {
                TempData["ErrorMessage"] = "You can no longer edit this request.";
                return RedirectToAction("Index");
            }

            var Phrasemodel = _context.phraseModel.Where(p => p.RequestMaintenanceID == id).ToList();
            item.phraseModel = Phrasemodel;
            var CheckboxesList = PhraseModel.PredefinedPhrases();
            foreach (var checkbox in CheckboxesList)
            {
                foreach (var option in item.phraseModel)
                {
                    if (checkbox.Phrase == option.Phrase && checkbox.Category == option.Category)
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            item.phraseModel = CheckboxesList;

            if (item == null)
            {
                return NotFound();
            }

            foreach (var option in item.phraseModel)
            {
                if (option.Category == "Request Status" && option.IsChecked)
                    item.RequestStatus = option.Phrase;
                if (option.Category == "Machine" && option.IsChecked)
                    item.MachineState = option.Phrase;
                if (option.Category == "Machine Status 2" && option.IsChecked)
                    item.MachineState2 = option.Phrase;
                if (option.Category == "Status of Request" && option.IsChecked)
                    item.StatufOfRequest = option.Phrase;

            }
           
            return View("Maintainer/Edit2", item);
        }

        [HttpGet]
        [Route("MaintenanceRequest/Maintainer/Edit/{id}")]
        public IActionResult MaintainerEdit(int id)
        {
            var item = _context.requestMaintenanceModel.Find(id);
            if (item.FormStatus == "Completed")
            {
                TempData["ErrorMessage"] = "You can no longer edit this request.";
                return RedirectToAction("Index");
            }
            var Phrasemodel = _context.phraseModel.Where(p => p.RequestMaintenanceID == id).ToList();
            item.phraseModel = Phrasemodel;
            var CheckboxesList = PhraseModel.PredefinedPhrases();
            foreach (var checkbox in CheckboxesList)
            {
                foreach (var option in item.phraseModel)
                {
                    if (checkbox.Phrase == option.Phrase && checkbox.Category == option.Category)
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            item.phraseModel = CheckboxesList;

            if (item == null)
            {
                return NotFound();
            }

            foreach (var option in item.phraseModel)
            {
                Debug.WriteLine(option.Phrase);

                if (option.Category == "Request Status" && option.IsChecked)
                    item.RequestStatus = option.Phrase;

                if (option.Category == "Machine" && option.IsChecked)
                    item.MachineState = option.Phrase;


            }
            /*foreach (var option in item.ReviewOfProblem)
            {
                if (item.ReviewOfProblem != null)
                    phrases.Add(new PhraseModel { Phrase = option, Category = "Review of Problems", IsChecked = true });
            }*/
            return View("Maintainer/Edit", item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MaintenanceRequest/Maintainer/SubmitEdit")]
        public async Task<IActionResult> MaintainerSubmitEdit(RequestMaintenanceModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    List<PhraseModel> phrases = new List<PhraseModel>();
                    List<PhraseModel> RequestFor = new List<PhraseModel>();
                    List<PhraseModel> RequestStatus = new List<PhraseModel>();
                    model.FormStatus = "Pending";

                    var existingRecord = await _context.requestMaintenanceModel.FindAsync(model.Id);

                    if (existingRecord == null)
                    {
                        // Handle the case where the record doesn't exist
                        return NotFound("The record you are trying to edit does not exist.");
                    }
                    existingRecord.ReviewRemarks = model.ReviewRemarks;
                    existingRecord.FormStatus = model.FormStatus;

                    foreach (var property in typeof(RequestMaintenanceModel).GetProperties())
                    {
                        ModelState.Remove(property.Name);
                    }


                    // 1. Delete the previous PhraseModel entries related to the model's Id
                    var previousPhrases = _context.phraseModel.Where(p => p.RequestMaintenanceID == model.Id && p.Category=="Review of Problems").ToList();

                    if (previousPhrases != null && previousPhrases.Any())
                    {
                        _context.phraseModel.RemoveRange(previousPhrases);
                    }

                    if (model.ReviewOfProblem.Count == 0)
                    {
                        ModelState.AddModelError(nameof(model.ReviewOfProblem), "At least one option must be selected.");
                    }
                    else
                    {
                        foreach (var option in model.ReviewOfProblem)
                        {
                            if (model.ReviewOfProblem != null)
                                phrases.Add(new PhraseModel { Phrase = option, Category = "Review of Problems", IsChecked = true });
                        }
                    }
                    if(model.ReviewRemarks == null)
                    {
                        ModelState.AddModelError(nameof(model.ReviewRemarks), "Review Remarks is required.");
                    }

                    // 2. Add the new PhraseModel entries
                    foreach (var selected in phrases)
                    {
                        selected.RequestMaintenanceID = model.Id;
                        _context.phraseModel.Add(selected);
                    }

                    // 3. Save changes to the database (both deletions and insertions)
                    await _context.SaveChangesAsync();

                    // 4. Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // If something fails, roll back the transaction
                    await transaction.RollbackAsync();
                    // Handle the exception or show an error message
                    return View("Error");
                }
            }
            //return Json(new { success = true, redirectUrl = Url.Action("Index", "MaintenanceRequest", new { tab = "Pending" }) });
            TempData["SuccessMessage"] = "Record saved successfully!";
            return RedirectToAction("Index", new { tab = "Pending" });

        }


        [HttpGet]
        [Route("MaintenanceRequest/Initiator/Edit/{id}")]
        public IActionResult InitiatorEdit(int id)
        {
            var item = _context.requestMaintenanceModel.Find(id);
            if (item.FormStatus == "Pending")
            {
                TempData["ErrorMessage"] = "You can no longer edit this request.";
                return RedirectToAction("Index");
            }
            var Phrasemodel = _context.phraseModel.Where(p => p.RequestMaintenanceID == id).ToList();
            item.phraseModel = Phrasemodel;
            var CheckboxesList = PhraseModel.PredefinedPhrases();
            foreach (var checkbox in CheckboxesList)
            {
                foreach (var option in item.phraseModel)
                {
                    if (checkbox.Phrase == option.Phrase && checkbox.Category == option.Category)
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            item.phraseModel = CheckboxesList;

            if (item == null)
            {
                return NotFound();
            }

            foreach (var option in item.phraseModel)
            {
                Debug.WriteLine(option.Phrase);

                if (option.Category == "Request Status" && option.IsChecked)
                    item.RequestStatus = option.Phrase;

                if (option.Category == "Machine" && option.IsChecked)
                    item.MachineState = option.Phrase;

            }
            return View("Initiator/Edit", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MaintenanceRequest/Initiator/SubmitEdit")]
        public async Task<IActionResult> SubmitEdit(RequestMaintenanceModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                   
                    List<PhraseModel> phrases = new List<PhraseModel>();
                    List<PhraseModel> RequestFor = new List<PhraseModel>();
                    List<PhraseModel> RequestStatus = new List<PhraseModel>();
                    model.FormStatus = "New";

                    

                    var existingRecord = await _context.requestMaintenanceModel.FindAsync(model.Id);

                    if (existingRecord == null)
                    {
                        // Handle the case where the record doesn't exist
                        return NotFound("The record you are trying to edit does not exist.");
                    }
                    existingRecord.RequestDate = model.RequestDate;
                    existingRecord.Department = model.Department;
                    existingRecord.RequestedBy = model.RequestedBy;
                    existingRecord.ReceivedBy = model.ReceivedBy;
                    existingRecord.ReceivedTime = model.ReceivedTime;
                    existingRecord.Machine = model.Machine;
                    existingRecord.Description = model.Description;
                    existingRecord.Proposal = model.Proposal;

                    ModelState.Remove(nameof(RequestMaintenanceModel.Proposal));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ReviewRemarks));
                    ModelState.Remove(nameof(RequestMaintenanceModel.DescriptionAction));
                    ModelState.Remove(nameof(RequestMaintenanceModel.MachineState2));
                    ModelState.Remove(nameof(RequestMaintenanceModel.RequestOpen));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionTakenBy));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionTakenDate));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionApprovedBy));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionApprovedDate));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionAcknowledgedBy));
                    ModelState.Remove(nameof(RequestMaintenanceModel.ActionAcknowledgedDate));

                    // 1. Delete the previous PhraseModel entries related to the model's Id
                    var previousPhrases = _context.phraseModel.Where(p => p.RequestMaintenanceID == model.Id).ToList();

                    if (previousPhrases != null && previousPhrases.Any())
                    {
                        _context.phraseModel.RemoveRange(previousPhrases);
                    }

                    if (model.RequestFor == null || !model.RequestFor.Any())
                    {
                        ModelState.AddModelError(nameof(model.RequestFor), "At least one option must be selected.");
                    }
                    else
                    {
                        foreach (var option in model.RequestFor)
                        {

                            if (model.RequestFor != null)
                                phrases.Add(new PhraseModel { Phrase = option, Category = "Request For", IsChecked = true });
                        }
                    }

                    if (model.MachineState == null)
                    {
                        ModelState.AddModelError(nameof(model.MachineState), "At least one option must be selected.");
                    }
                    else
                    {
                        phrases.Add(new PhraseModel { Phrase = model.MachineState, Category = "Machine", IsChecked = true });

                    }
                    if (model.RequestStatus == null)
                    {
                        ModelState.AddModelError(nameof(model.MachineState), "At least one option must be selected.");
                    }
                    else
                    {
                        phrases.Add(new PhraseModel { Phrase = model.RequestStatus, Category = "Request Status", IsChecked = true });
                    }


                    // 2. Add the new PhraseModel entries
                    foreach (var selected in phrases)
                    {
                        selected.RequestMaintenanceID = model.Id;
                        _context.phraseModel.Add(selected);
                    }

                    // 3. Save changes to the database (both deletions and insertions)
                    await _context.SaveChangesAsync();

                    // 4. Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // If something fails, roll back the transaction
                    await transaction.RollbackAsync();
                    // Handle the exception or show an error message
                    return View("Error");
                }
            }
            return RedirectToAction("Index", new { tab = "New" });

        }

        [HttpGet]
        [Route("MaintenanceRequest/Initiator/Create")]
        public IActionResult InitiatorCreate()
        {
            var model = new RequestMaintenanceModel();
            model.RequestFor.Add("Mechanical");
            model.RequestFor.Add("Electrical");
            model.RequestFor.Add("Electronic");

            model.RequestDate = DateOnly.FromDateTime(DateTime.Now);
            model.ReceivedTime = TimeOnly.FromDateTime(DateTime.Now);


            return View("Initiator/Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MaintenanceRequest/Initiator/Create")]
        public async Task<IActionResult> InitiatorCreate(RequestMaintenanceModel model)
        {

            List<PhraseModel> phrases = new List<PhraseModel>();

            List<PhraseModel> RequestFor = new List<PhraseModel>();
            List<PhraseModel> MachineState = new List<PhraseModel>();
            List<PhraseModel> RequestStatus = new List<PhraseModel>();
            model.FormStatus = "New";

            

            foreach (var option in model.RequestFor)
            {
                phrases.Add(new PhraseModel { Phrase = option, Category = "Request For", IsChecked = true });
            }
            if (model.RequestFor.Count == 0)
            {
                ModelState.AddModelError(nameof(model.RequestFor), "At least one option must be selected.");
            }
            //if (model.ReviewOfProblem != null && model.ReviewOfProblem.Any())
            //{
            //    foreach (var option in model.ReviewOfProblem)
            //    {
            //        if (model.ReviewOfProblem != null)
            //            phrases.Add(new PhraseModel { Phrase = option, Category = "Review of Problems", IsChecked = true });
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("ReviewOfProblem", "At least one option must be selected.");
            //}


            phrases.Add(new PhraseModel { Phrase = model.MachineState, Category = "Machine", IsChecked = true });
            phrases.Add(new PhraseModel { Phrase = model.RequestStatus, Category = "Request Status", IsChecked = true });


            _context.requestMaintenanceModel.Add(model);
            await _context.SaveChangesAsync();

            foreach (var selected in phrases)
            {
                selected.RequestMaintenanceID = model.Id;
                _context.phraseModel.Add(selected);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new {tab = "New"});

        }



        [HttpPost]
        public IActionResult GetRequests(string FormStatus)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault(); // asc or desc

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Get column name to sort by
                var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();

                // Query Data
                var customerData = _context.requestMaintenanceModel.AsQueryable();

                // Apply Search Filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.Id.ToString().Contains(searchValue) ||
                        m.Department.Contains(searchValue) ||
                        m.RequestDate.ToString().Contains(searchValue) ||
                        m.RequestedBy.Contains(searchValue) ||
                        m.ReceivedBy.Contains(searchValue));
                }

                // Apply Sorting
                if (!string.IsNullOrEmpty(sortColumnName))
                {
                    switch (sortColumnName)
                    {
                        case "ID":
                            customerData = sortDirection == "desc" ? customerData.OrderBy(x => x.Id) : customerData.OrderByDescending(x => x.Id);
                            break;
                        case "RequestDate":
                            customerData = sortDirection == "desc" ? customerData.OrderBy(x => x.RequestDate) : customerData.OrderByDescending(x => x.RequestDate);
                            break;
                        case "Department":
                            customerData = sortDirection == "asc" ? customerData.OrderBy(x => x.Department) : customerData.OrderByDescending(x => x.Department);
                            break;
                        case "RequestedBy":
                            customerData = sortDirection == "asc" ? customerData.OrderBy(x => x.RequestedBy) : customerData.OrderByDescending(x => x.RequestedBy);
                            break;
                        case "ReceivedBy":
                            customerData = sortDirection == "asc" ? customerData.OrderBy(x => x.ReceivedBy) : customerData.OrderByDescending(x => x.ReceivedBy);
                            break;
                        default:
                            customerData = customerData.OrderBy(x => x.Id); // Default sort
                            break;
                    }
                }

                // Total Records Count
                recordsTotal = customerData.Count();

                var data = customerData.Select(n => new { n.Id, n.RequestDate, n.Department, n.RequestedBy, n.ReceivedBy, n.FormStatus }).Take(1).ToList();
                data.Clear();
                // Paginate Data
                if (FormStatus == "All")
                {
                    data = customerData.Select(n => new { n.Id, n.RequestDate, n.Department, n.RequestedBy, n.ReceivedBy, n.FormStatus }).Skip(skip).Take(pageSize).ToList();

                }
                else
                {
                    data = customerData.Where(p => p.FormStatus == FormStatus).Select(n => new { n.Id, n.RequestDate, n.Department, n.RequestedBy, n.ReceivedBy, n.FormStatus }).Skip(skip).Take(pageSize).ToList();

                }

                // JSON Response
                var jsonData = new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new RequestMaintenanceModel();
            model.RequestFor.Add("Mechanical");
            model.RequestFor.Add("Electrical");
            model.RequestFor.Add("Electronic");
            model.ReviewOfProblem.Add("Repair at the spot");
            model.ReviewOfProblem.Add("Part Replacement");
            model.ReviewOfProblem.Add("By-pass");
            model.ReviewOfProblem.Add("Adjustment");
            model.ReviewOfProblem.Add("Part to be purchased");
            model.ReviewOfProblem.Add("Problem forwarded to Maintenance Contractor");
            model.ReviewOfProblem.Add("Part to be fabricated from workshop");
            model.ReviewOfProblem.Add("Part repair / fabricated from outside");
            model.ReviewOfProblem.Add("Modifying / New Job");

            return View(model);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            
            var item = _context.requestMaintenanceModel.Find(id);
            var Phrasemodel = _context.phraseModel.Where(p => p.RequestMaintenanceID == id).ToList();
            item.phraseModel = Phrasemodel;
            var CheckboxesList = PhraseModel.PredefinedPhrases();
            foreach (var checkbox in CheckboxesList)
            {
                foreach(var option in item.phraseModel)
                {
                    if(checkbox.Phrase == option.Phrase && checkbox.Category == option.Category)
                    {
                        checkbox.IsChecked = true;
                    }                   
                }
            }
            item.phraseModel = CheckboxesList;

            if (item == null)
            {
                return NotFound();
            }

            foreach (var option in item.phraseModel)
            {
                Debug.WriteLine(option.Phrase);

                if (option.Category == "Request Status" && option.IsChecked)
                    item.RequestStatus = option.Phrase;

                if (option.Category == "Machine" && option.IsChecked)
                    item.MachineState = option.Phrase;

                if (option.Category == "Machine Status 2" && option.IsChecked)
                    item.MachineState2 = option.Phrase;

                if (option.Category == "Status of Request" && option.IsChecked)
                    item.StatufOfRequest = option.Phrase;

            }


            return View(item);
        }

        [HttpGet]
        public IActionResult Index(int position=0) {
            var model = _context.requestMaintenanceModel
        .OrderByDescending(p => p.Id)
        .Skip(position)
        .Take(10)
        .ToList(); 

            int totalItems = _context.requestMaintenanceModel.Count();
            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPosition = position;
            Debug.WriteLine($"Model count: {model.Count}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestMaintenanceModel model)
        {

            List<PhraseModel> phrases = new List<PhraseModel>();

            List<PhraseModel> RequestFor = new List<PhraseModel>();
            List<PhraseModel> MachineState = new List<PhraseModel>();
            List<PhraseModel> MachineState2 = new List<PhraseModel>();
            List<PhraseModel> RequestStatus = new List<PhraseModel>();
            List<PhraseModel> ReviewOfProblems = new List<PhraseModel>();
            model.FormStatus = "New";

            if (model.RequestFor != null && model.RequestFor.Any())
            {
                foreach (var option in model.RequestFor)
                {
                    phrases.Add(new PhraseModel { Phrase = option, Category = "Request For", IsChecked = true });
                }
            }
            else
            {
                ModelState.AddModelError("RequestFor", "At least one option must be selected.");
            }

            if (model.ReviewOfProblem != null && model.ReviewOfProblem.Any())
            {
                foreach (var option in model.ReviewOfProblem)
                {
                    if (model.ReviewOfProblem != null)
                        phrases.Add(new PhraseModel { Phrase = option, Category = "Review of Problems", IsChecked = true });
                }
            }
            else
            {
                ModelState.AddModelError("ReviewOfProblem", "At least one option must be selected.");
            }

            
            phrases.Add(new PhraseModel { Phrase = model.MachineState, Category = "Machine", IsChecked = true });
            phrases.Add(new PhraseModel { Phrase = model.MachineState2, Category = "Machine Status 2", IsChecked = true });
            phrases.Add(new PhraseModel { Phrase = model.RequestStatus, Category = "Request Status", IsChecked = true });
            phrases.Add(new PhraseModel { Phrase = model.StatufOfRequest, Category = "Status of Request", IsChecked = true });


            _context.requestMaintenanceModel.Add(model);
            await _context.SaveChangesAsync();
          
                foreach (var selected in phrases)
                {
                    selected.RequestMaintenanceID = model.Id;
                    _context.phraseModel.Add(selected);
                }
           
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
           
        }
        
    }
}
