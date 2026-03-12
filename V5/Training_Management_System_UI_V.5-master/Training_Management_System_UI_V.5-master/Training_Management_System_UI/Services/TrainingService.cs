    using Microsoft.AspNetCore.Components.WebAssembly.Http;
    using System.Net.Http.Json;
    using Training_Management_System_UI.Models.Training;
    using Microsoft.AspNetCore.Components.Forms;

namespace Training_Management_System_UI.Services
    {
        public class TrainingService
        {
            private readonly HttpClient _http;

            public TrainingService(HttpClient http)
            {
                _http = http;
            }

            public async Task<List<TrainingResponse>> GetAllTrainingsAsync()
            {
                try
                {
                    var result = await _http.GetFromJsonAsync<List<TrainingResponse>>("api/training/trainings");
                    return result ?? new List<TrainingResponse>();
                }
                catch (Exception)
                {
                    return new List<TrainingResponse>();
                }
            }

            // CREATE TRAINING
            public async Task<(bool Success, string Message)> CreateTrainingAsync(
                string title,
                string description,
                int durationInDays,
                Guid createdByUserId,
                List<StagedFile> files,         // ← changed from IBrowserFile
                List<AddAttendeeItem> attendees)
            {
                try
                {
                    var form = new MultipartFormDataContent();

                    form.Add(new StringContent(title), "Title");
                    form.Add(new StringContent(description), "Description");
                    form.Add(new StringContent(durationInDays.ToString()), "TrainingDurationInDays");
                    form.Add(new StringContent(createdByUserId.ToString()), "CreatedByUserId");

                    for (int i = 0; i < attendees.Count; i++)
                    {
                        form.Add(new StringContent(attendees[i].FullName), $"Attendees[{i}].FullName");
                        form.Add(new StringContent(attendees[i].Email), $"Attendees[{i}].Email");
                        form.Add(new StringContent(attendees[i].Contact), $"Attendees[{i}].Contact");
                    }

                    // ← now uses byte[] instead of IBrowserFile
                    for (int i = 0; i < files.Count; i++)
                    {
                        var fileContent = new ByteArrayContent(files[i].Data);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                        form.Add(fileContent, $"Materials[{i}].File", files[i].Name);
                        form.Add(new StringContent("false"), $"Materials[{i}].IsExternal");
                    }

                    var request = new HttpRequestMessage(HttpMethod.Post, "api/training/create");
                    request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                    request.Content = form;

                    var response = await _http.SendAsync(request);
                    var responseMessage = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return (true, "Training created successfully!");

                    return (false, responseMessage);
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }
            }

        //  MANAGE TRAININGS
        public async Task<(bool Success, string Message)> DeleteTrainingAsync(Guid trainingId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"api/training/delete/{trainingId}");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                var response = await _http.SendAsync(request);
                var message = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> ToggleDisabledAsync(Guid trainingId, bool disabled)
        {
            try
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(disabled.ToString()), "Disabled");

                var request = new HttpRequestMessage(HttpMethod.Put, $"api/training/update/{trainingId}");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                request.Content = form;

                var response = await _http.SendAsync(request);
                var message = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public class AddAttendeeItem
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Contact { get; set; } = string.Empty;
        }

        public class StagedFile
        {
            public string Name { get; set; } = string.Empty;
            public string ContentType { get; set; } = string.Empty;
            public long Size { get; set; }
            public byte[]? Data { get; set; }
            public IBrowserFile? BrowserFile { get; set; }
        }

        // ================================================================ 
        // ADD THESE TWO METHODS inside the TrainingService class,
        // after the existing ToggleDisabledAsync method.
        // ================================================================

        public async Task<(bool Success, string Message)> AddMaterialsAsync(
            Guid trainingId,
            List<StagedFile> files)
        {
            try
            {
                var form = new MultipartFormDataContent();

                for (int i = 0; i < files.Count; i++)
                {
                    var fileContent = new ByteArrayContent(files[i].Data);
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                    form.Add(fileContent, $"Materials[{i}].File", files[i].Name);
                    form.Add(new StringContent("false"), $"Materials[{i}].IsExternal");
                }

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"api/training/{trainingId}/materials");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                request.Content = form;

                var response = await _http.SendAsync(request);
                var message = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> AddAttendeeAsync(
            Guid trainingId,
            AddAttendeeItem attendee)
        {
            try
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(attendee.FullName), "FullName");
                form.Add(new StringContent(attendee.Email), "Email");
                form.Add(new StringContent(attendee.Contact), "Contact");

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"api/training/{trainingId}/attendees");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                request.Content = form;

                var response = await _http.SendAsync(request);
                var message = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateTrainingAsync(
            Guid trainingId,
            string title,
            string description,
            int durationInDays,
            List<AttendeeResponse>? existingAttendees = null,
            List<MaterialResponse>? existingMaterials = null)
                {
                    try
                    {
                        var form = new MultipartFormDataContent();
                        form.Add(new StringContent(trainingId.ToString()), "TrainingId");
                        form.Add(new StringContent(title), "Title");
                        form.Add(new StringContent(description), "Description");
                        form.Add(new StringContent(durationInDays.ToString()), "TrainingDurationInDays");

                        // Send existing attendees back so backend doesn't wipe them
                        if (existingAttendees != null)
                        {
                            for (int i = 0; i < existingAttendees.Count; i++)
                            {
                                form.Add(new StringContent(existingAttendees[i].AttendeeId.ToString()), $"UpdateAttendee[{i}].AttendeeId");
                                form.Add(new StringContent(trainingId.ToString()), $"UpdateAttendee[{i}].TrainingId");
                                form.Add(new StringContent(existingAttendees[i].FullName), $"UpdateAttendee[{i}].Name");
                                form.Add(new StringContent(existingAttendees[i].Email), $"UpdateAttendee[{i}].Email");
                                form.Add(new StringContent(existingAttendees[i].Contact ?? ""), $"UpdateAttendee[{i}].Contact");
                            }
                        }

                        // Send existing materials back so backend doesn't wipe them
                        if (existingMaterials != null)
                        {
                            for (int i = 0; i < existingMaterials.Count; i++)
                            {
                                form.Add(new StringContent(existingMaterials[i].MaterialId.ToString()), $"UpdateMaterials[{i}].MaterialId");
                                form.Add(new StringContent(trainingId.ToString()), $"UpdateMaterials[{i}].TrainingId");
                                form.Add(new StringContent("false"), $"UpdateMaterials[{i}].IsExternal");
                            }
                        }

                        var request = new HttpRequestMessage(HttpMethod.Put, $"api/training/update/{trainingId}");
                        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                        request.Content = form;

                        var response = await _http.SendAsync(request);
                        var message = await response.Content.ReadAsStringAsync();
                        return (response.IsSuccessStatusCode, message);
                    }
                    catch (Exception ex)
                    {
                        return (false, ex.Message);
                    }
                }
        }
    }