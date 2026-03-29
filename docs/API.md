# MCIApi – API Documentation

This document describes all controllers in the MCIApi solution. Each section lists the available endpoints with their purpose, parameters, sample payloads/responses, authorization requirements, and any relevant notes.

Common routing pattern: most controllers use `api/{lang}/[controller]`, where `{lang}` must be `en` or `ar`. Unless noted otherwise, requests require a valid Bearer token (check controller attributes for `[Authorize]`).

---

## 1. AccountController (`api/{lang}/Account`)
Handles user authentication, registration, OTP, and logout. Anonymous access for most endpoints except `logout`, which requires a Bearer token.

### POST `/register`
- **Description:** Registers a new user.
- **Authorization:** Anonymous.
- **Parameters:**
  - Route: `lang` (`en`/`ar`).
  - Body: `RegisterRequestDto`.
- **Request Body Schema (RegisterRequestDto):**
  ```json
  {
    "firstName": "string",
    "lastName": "string",
    "email": "user@example.com",
    "phoneNumber": "+201234567890",
    "password": "P@ssw0rd!",
    "confirmPassword": "P@ssw0rd!"
  }
  ```
- **Example Request:**
  ```http
  POST /api/en/Account/register
  Content-Type: application/json
  
  {
    "firstName": "Sara",
    "lastName": "Ibrahim",
    "email": "sara@mci.com",
    "phoneNumber": "+201234567890",
    "password": "Admin@12345",
    "confirmPassword": "Admin@12345"
  }
  ```
- **Success Response (200):**
  ```json
  { "message": "User registered successfully." }
  ```
- **Error Responses:**
  - 400 (validation) with localized field errors.
  - 400 (identity) with `{ "code": "DuplicateUserName", "description": "..." }`.
- **Notes:** Localized messages via `ILocalizationHelper`. Password rules enforced (length, digits, case, special chars).

### POST `/login`
- **Description:** Generates a JWT for valid credentials.
- **Authorization:** Anonymous.
- **Body:** `LoginRequestDto` (`email`/`password`).
- **Success (200):** `{ "token": "<jwt>" }`.
- **Errors:** 400 (validation), 401 invalid credentials.
- **Notes:** Token is required for protected endpoints.

### POST `/send-otp`
- **Description:** Sends OTP SMS to a valid Egyptian mobile number.
- **Authorization:** Anonymous.
- **Query Params:** `phoneNumber` (must match `^(?:\+201|01|1)[0-2,5]{1}[0-9]{8}$`).
- **Success (200):** `{ "message": "OTP sent." }`.
- **Errors:** 400 invalid format, 404 user not found.

### POST `/verify-otp`
- **Body:** `VerifyOtpDto` (phone, otp code).
- **Success:** `{ "message": "OTP verified." }`.
- **Error:** 400 invalid/expired OTP.

### POST `/reset-password`
- **Body:** `ResetPasswordOnlyDto` (email, new password).
- **Success:** `{ "message": "Password reset successfully." }`.
- **Errors:** 400 validation or identity errors.

### POST `/logout`
- **Authorization:** Requires Bearer token in `Authorization` header.
- **Success:** `{ "message": "Logged out successfully." }`.
- **Errors:** 400 token missing, 401 invalid/expired token.
- **Notes:** Token is invalidated via blacklist service.

---

## 2. BatchController (`api/{lang}/Batch`)
Manages batches used for claims/payables. Requires authentication (`[Authorize]`).

### GET `/`
- **Description:** Retrieves paginated batches.
- **Query Params:** `page` (default 1), `limit` (default 10).
- **Success (200):**
  ```json
  {
    "totalBatches": 35,
    "currentPage": 1,
    "limit": 10,
    "totalPages": 4,
    "data": [ { "id": 1, "batchNumber": "BCH-001", ... } ]
  }
  ```
- **Notes:** Response data structure defined by `BatchPagedResultDto`.

### GET `/{id}`
- **Description:** Returns batch by ID.
- **Success:** `BatchReadDto` object.
- **404:** `{ "message": "Batch not found" }`.

### POST `/`
- **Description:** Creates a batch.
- **Body:** `BatchCreateDto` (e.g., `providerId`, `invoiceNumber`, `issueDate`, `amount`, attachment files).
- **Success (200):** Returns created batch DTO.
- **Errors:** 400 invalid model/unexpected errors.
- **Notes:** Uses current employee ID from token for auditing.

### PUT `/{id}`
- **Description:** Updates a batch.
- **Body:** `BatchUpdateDto`.
- **Success:** Updated batch DTO.
- **Errors:** 404 not found, 400 validation/conflict.

### DELETE `/{id}`
- **Description:** Soft-deletes a batch.
- **Success:** `{ "success": true }` (per service result).
- **Error:** 404 batch not found.

---

## 3. BranchController (`api/{lang}/Branch`)
Handles client branch records (with optional image uploads).

### GET `/`
- **Query:** `page` (default 1), `limit` (default 5).
- **Success (200):**
  ```json
  {
    "totalBranches": 12,
    "currentPage": 1,
    "limit": 5,
    "totalPages": 3,
    "data": [ { "branchId": 10, "branchName": "Cairo" } ]
  }
  ```
- **Notes:** Data from `BranchPagedResultDto` (`CreateBranchDto` uses `multipart/form-data`).

### GET `/{id}`
- **Success:** `BranchReadDto`.
- **Errors:** 404 with localized `BranchNotFound` message.

### POST `/`
- **Body:** `CreateBranchDto` (multipart form: text fields + optional `ImageFile`).
- **Success (201):**
  ```json
  {
    "id": 5,
    "clientId": 3,
    "clientName": "ABC Corp",
    "branchName": "Giza"
  }
  ```
- **Errors:** 400 validation/unexpected (localized messages).

### PUT `/{id}`
- **Body:** `UpdateBranchDto` (multipart form).
- **Success:** 204 No Content.
- **Errors:**
  - 400 invalid status (must be `Active` or `DeActive`).
  - 404 branch not found.

### DELETE `/{id}`
- **Success:** 204 No Content.
- **Error:** 404 branch not found.

---

## 4. CategoryController (`api/{lang}/Category`)
CRUD for categories.

### GET `/`
- **Description:** Returns all categories.
- **Success:** `CategoryReadDto[]`.

### GET `/{id}`
- **Success:** Single category.
- **404:** Localized `NotFound` message.

### POST `/`
- **Body:** `CategoryCreateDto` (`name` required, 2–200 chars).
- **Success (201):** Created category.
- **Errors:**
  - 400 validation.
  - 400 conflict if name exists (`CategoryNameExists`).

### PUT `/{id}`
- **Body:** `CategoryUpdateDto`.
- **Success:**
  ```json
  {
    "message": "Category updated",
    "category": { "id": 1, "name": "Updated" }
  }
  ```
- **Errors:** 400 conflict/validation, 404 not found.

### DELETE `/{id}`
- **Success:** `{ "message": "Category deleted" }`.
- **Errors:** 404 not found.

- **Authorization:** Typically requires authenticated user (depends on global policies; controller itself has no `[Authorize]`, so confirm pipeline configuration).
- **Notes:** Localization via `ILocalizationHelper`.

---

## 5. ClaimController (`api/{lang}/Claim`)
Manages claim submissions, attachments, and status. (Authorization must be handled by higher-level middleware/policies.)

### GET `/`
- **Query:** `page` (default 1), `limit` (default 10).
- **Success:**
  ```json
  {
    "totalClaims": 18,
    "currentPage": 1,
    "limit": 10,
    "totalPages": 2,
    "data": [ { "id": 1, "batchId": 5, ... } ]
  }
  ```

### GET `/{id}`
- **Success:** Claim details (`ClaimReadDto`).
- **Error:** 404 claim not found.

### POST `/`
- **Body:** `ClaimCreateDto` via `multipart/form-data` (supports files; `RequestSizeLimit(50MB)`). Includes `BatchId`, `MemberName`, `ServiceDate`, `Amount`, attachments, etc.
- **Success:** Created claim object.
- **Errors:** 400 invalid model/batch.

### PUT `/{id}`
- **Body:** `ClaimUpdateDto` (multipart form for attachments).
- **Success:** Updated claim.
- **Errors:** 404 not found, 400 validation/unexpected.

### DELETE `/{id}`
- **Success:** `{ "success": true }`.
- **Error:** 404 not found.

- **Notes:** Claims rely on existing batches; service returns `ServiceErrorType.NotFound` when dependencies are missing.

---

## 6. ClientController (`api/{lang}/Client`)
Manages client companies and their metadata/images. Accepts `multipart/form-data` for create/update when uploading logos.

### GET `/`
- **Description:** Paginated client list with search and filter capabilities.
- **Query Params:** 
  - `page` (default 1)
  - `limit` (default 5)
  - `searchColumn` (optional): Column to search in (e.g., "id", "arabicname", "englishname", "shortname", "category", "type", "status")
  - `search` (optional): Search term (searches in all columns if `searchColumn` is not provided)
  - `statusId` (optional): Filter by status ID
- **Success:**
  ```json
  {
    "totalClients": 24,
    "currentPage": 1,
    "limit": 5,
    "totalPages": 5,
    "data": [ 
      { 
        "Id": 3, 
        "ArabicName": "شركة ABC", 
        "EnglishName": "ABC Corp", 
        "Category": "Gold",
        "Type": "Corporate", 
        "Status": "Active",
        "Branches": 5,
        "Members": 150
      } 
    ]
  }
  ```
- **Authorization:** None explicitly (follows global policies).
- **Notes:** Response uses PascalCase property names. Search is case-insensitive and supports wildcard search across all columns if `searchColumn` is omitted.

### GET `/{id}`
- **Description:** Returns a client with full image URL and all details including LevelId and LevelName.
- **Success:`
  ```json
  {
    "Id": 3,
    "ArabicName": "شركة ABC",
    "EnglishName": "ABC Corp",
    "ShortName": "ABC",
    "CategoryId": 1,
    "CategoryName": "Gold",
    "TypeId": 2,
    "TypeName": "Corporate",
    "StatusId": 1,
    "StatusName": "Active",
    "LevelId": 1,
    "LevelName": "Premium",
    "RefundDueDays": 10,
    "PolicyId": 5,
    "PolicyStart": "2024-01-01",
    "PolicyExpire": "2024-12-31",
    "ImageUrl": "https://api.mci.com/uploads/clients/3.png",
    "Contacts": [],
    "Branches": [],
    "Contracts": [],
    "Members": []
  }
  ```
- **Error:** 404 with localized `ClientNotFound` message.

### POST `/`
- **Body:** `ClientCreateDto` (`multipart/form-data`). Key rules:
  - `ArabicName` Arabic letters 2–200 chars.
  - `EnglishName` alphabetic 2–200 chars.
  - `Type` ∈ {`Groups`,`Corporate`,`Individual`,`Cash Card`}.
  - `RefundDueDays` 1–15.
  - `ImageFile` optional (<=2MB, jpg/png).
- **Success (201):** returns created DTO (`Id`, `Name`, etc.).
- **Errors:** 400 validation or localized unexpected errors.

### PUT `/{id}`
- **Body:** `ClientUpdateDto` (multipart). Allows partial updates, toggling status, replacing/deleting image. Supports `LevelId` update.
- **Success:** `{ "message": "Client updated", "id": 3 }`.
- **Errors:** 404 not found, 400 validation/unexpected.

### PATCH `/{id}/status`
- **Description:** Updates client status by StatusId.
- **Body:** `ClientStatusUpdateDto` with `StatusId` (required, int).
- **Success (200):** `{ "message": "Client status updated" }`.
- **Errors:** 
  - 404: Client not found
  - 400: Status not found or validation error

### DELETE `/{id}`
- **Success:** 204 No Content.
- **Error:** 404 not found.

### GET `/statuses`
- **Description:** Returns all available statuses for lookup.
- **Success (200):** 
  ```json
  [
    { "Id": 1, "Name": "Activated" },
    { "Id": 2, "Name": "Deactivated" },
    { "Id": 3, "Name": "Hold" },
    { "Id": 4, "Name": "Pending" }
  ]
  ```
- **Notes:** Names are localized based on `lang` parameter.

### GET `/types`
- **Description:** Returns all client types for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "Corporate" }, ... ]`
- **Notes:** Names are localized (Arabic/English).

### GET `/categories`
- **Description:** Returns all categories for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "Gold" }, ... ]`

### GET `/insurance-companies`
- **Description:** Returns all insurance companies for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "Allianz" }, ... ]`
- **Notes:** Names are localized.

### GET `/programs`
- **Description:** Returns all programs for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "Basic Plan" }, ... ]`
- **Notes:** Names are localized.

### GET `/levels`
- **Description:** Returns all member levels for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "Premium" }, ... ]`
- **Notes:** Names are localized.

### GET `/vip-statuses`
- **Description:** Returns all VIP statuses for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "VIP" }, ... ]`
- **Notes:** Names are localized.

### GET `/export/excel`
- **Description:** Exports all clients to Excel file with optional search and filter.
- **Query Params:**
  - `searchColumn` (optional): Column to search in
  - `search` (optional): Search term
  - `statusId` (optional): Filter by status ID
- **Success (200):** Downloads Excel file (`Clients_YYYYMMDDHHMMSS.xlsx`)
- **Content-Type:** `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **Excel Columns:**
  - Arabic Name / الاسم العربي
  - English Name / الاسم الإنجليزي
  - Short Name / الاسم المختصر
  - Category / الفئة
  - Type / النوع
  - Status / الحالة
  - Branches Count / عدد الفروع
  - Members Count / عدد الأعضاء
- **Notes:** Headers are localized based on `lang` parameter. File excludes ID and Level columns.

- **Notes:** Image URLs are rewritten to absolute paths using the current request scheme/host. Response uses PascalCase property names.

---

## 7. DepartmentsController (`api/{lang}/Departments`)
CRUD for departments; responses localized.

### GET `/`
- **Description:** Returns all departments.
- **Success:** `[ { "id": 1, "nameAr": "الموارد البشرية", "nameEn": "HR" }, ... ]`.

### GET `/{id}`
- **Success:`
  ```json
  { "id": 2, "name": "HR" }
  ```
- **Errors:** 404 `DepartmentNotFound`.

### POST `/`
- **Body:** `DepartmentCreateDto` with `NameAr`, `NameEn` (required, length validations).
- **Success (201):** wraps message + created DTO.
- **Errors:** 400 validation/unexpected.

### PUT `/{id}`
- **Body:** `DepartmentCreateDto` reused for update.
- **Success:** `{ "message": "Department updated successfully" }`.
- **Errors:** 404 not found, 400 validation/unexpected.

### DELETE `/{id}`
- **Success:** `{ "message": "Department deleted successfully" }`.
- **Errors:** 404 not found.

- **Authorization:** Not decorated; rely on global security configuration.

---

## 8. EmployeeAuthController (`api/{lang}/EmployeeAuth`)
Dedicated login/logout for employees (mobile app/staff portal).

### POST `/login`
- **Body:** `EmployeeLoginDto` with `mobile`/`password`.
- **Success (200):`
  ```json
  { "token": "<jwt>", "employeeId": 101, "mobile": "+201234567890" }
  ```
- **Errors:** 400 validation, 401 invalid credentials.
- **Notes:** No `[Authorize]`; used to obtain employee token.

### POST `/logout`
- **Authorization:** Requires `Authorization: Bearer <token>`.
- **Success:** `{ "message": "Logout successful." }`.
- **Errors:** 400 token missing, 401 invalid/expired token.

---

## 9. EmployeeController (`api/{lang}/Employee`)
Handles employee CRUD with image uploads.

### GET `/`
- **Description:** Lists employees with department/job names and absolute image URLs.
- **Success:** `[ { "id": 5, "firstName": "Omar", "departmentName": "HR", "imageUrl": "https://.../employees/5.png" }, ... ]`.

### GET `/{id}`
- **Success:** Employee DTO with `ImageUrl` rewritten.
- **Errors:** 404 `EmployeeNotFound`.

### POST `/`
- **Body:** `CreateEmployeeDto` (`multipart/form-data`), fields include personal info, department/job IDs, optional `ImageFile` (size/type limits enforced via validation attributes).
- **Success (201):** Created employee DTO with full image URL.
- **Errors:**
  - 400 validation.
  - 400 identity errors (returns array `{ code, description }`).

### PUT `/{id}`
- **Body:** `UpdateEmployeeDto` (multipart) for partial updates + image replacement/removal.
- **Success:** Updated DTO with absolute image.
- **Errors:** 404 not found, 400 validation/identity errors.

### DELETE `/{id}`
- **Success:** `{ "message": "Employee deleted successfully", "employee": { ... } }`.
- **Errors:** 404 not found.

- **Authorization:** None specified on controller; follow global policy.
- **Notes:** Helper converts stored relative image paths to full URLs via current request.

---

## 10. GeneralProgramsController (`api/{lang}/GeneralPrograms`)
Manages insurance programs linked to policies.

### GET `/`
- **Description:** List all general programs.
- **Success:** `GeneralProgramReadDto[]`.

### GET `/{id}`
- **Success:** Returns program DTO.
- **Errors:** 404 `GeneralProgramNotFound`.

### POST `/`
- **Body:** `GeneralProgramCreateDto` (requires `PolicyId`, Arabic/English names, etc.).
- **Success (201):** Created DTO.
- **Errors:** 400 validation or business errors (e.g., invalid policy).

### PUT `/{id}`
- **Body:** `GeneralProgramUpdateDto`.
- **Success:** `{ "message": "General program updated." }`.
- **Errors:** 400 validation, 404 not found.

### DELETE `/{id}`
- **Success:** `{ "message": "General program deleted." }`.
- **Errors:** 404 not found.

- **Notes:** `HandleError` distinguishes validation vs not-found vs unexpected to return localized messages.

---

## 11. InsuranceCompanyController (`api/{lang}/InsuranceCompany`)
CRUD for insurance carriers. Inherits `BaseApiController` (authorization required) but GET actions allow anonymous access.

### GET `/`
- **Auth:** Anonymous.
- **Description:** Lists all insurance companies; image URLs rewritten to absolute paths.
- **Success:** `[ { "id": 1, "name": "Allianz", "imageUrl": "https://api.mci.com/uploads/insurers/1.png" } ]`.

### GET `/{id}`
- **Auth:** Anonymous.
- **Success:** `InsuranceCompanyReadDto` with absolute image URL.
- **Errors:** 404 `InsuranceCompanyNotFound`.

### POST `/`
- **Body:** `InsuranceCompanyCreateDto` (multipart form with `Name`, optional `ImageFile` <=2MB, jpg/png).
- **Success (201):**
  ```json
  {
    "id": 7,
    "name": "MedLife",
    "imageUrl": "https://api.mci.com/uploads/insurers/7.png",
    "message": "Insurance company created"
  }
  ```
- **Errors:** 400 validation/unexpected.

### PUT `/{id}`
- **Body:** `InsuranceCompanyUpdateDto` (multipart) to edit metadata/logo.
- **Success:** `{ "message": "Insurance company updated", "name": "MedLife", "imageUrl": "https://..." }`.
- **Errors:** 404 not found.

### DELETE `/{id}`
- **Success:** `{ "message": "Insurance company deleted" }`.
- **Errors:** 404 not found.

- **Auth:** Create/Update/Delete require Bearer token (inherited `[Authorize]`).

---

## 12. JobTitlesController (`api/{lang}/JobTitles`)
CRUD for job titles with localized responses.

### GET `/`
- **Description:** Returns all job titles.
- **Success:** `[ { "id": 1, "nameAr": "طبيب", "nameEn": "Doctor" } ]`.

### GET `/{id}`
- **Success:**
  ```json
  { "id": 5, "name": "Doctor" }
  ```
- **Errors:** 404 `JobTitleNotFound`.

### POST `/`
- **Body:** `CreateJobTitleDto` (Arabic/English names required, 2–100 chars).
- **Success (201):** Created DTO.
- **Errors:** 400 validation/unexpected.

### PUT `/{id}`
- **Body:** `UpdateJobTitleDto`.
- **Success:** 204 No Content.
- **Errors:** 404 not found, 400 validation.

### DELETE `/{id}`
- **Success:** `{ "message": "Job title deleted successfully" }`.
- **Errors:** 404 not found.

---

## 13. MemberInfoController (`api/{lang}/MemberInfo`)
Handles member info CRUD with file uploads and lookup endpoints. No `[Authorize]` attribute (follows global policies).

### GET `/`
- **Description:** Paginated member list with search and filter capabilities.
- **Query Params:**
  - `page` (default 1)
  - `limit` (default 10)
  - `searchColumn` (optional): Column to search in (e.g., "id", "name", "mobile", "clientname", "branchname", "programname", "statusname")
  - `search` (optional): Search term (searches in all columns if `searchColumn` is not provided)
  - `statusId` (optional): Filter by status ID
- **Success:**
  ```json
  {
    "TotalMembers": 120,
    "CurrentPage": 1,
    "Limit": 10,
    "TotalPages": 12,
    "Data": [ 
      { 
        "Id": 10, 
        "BirthDate": "1990-05-15",
        "Age": 33,
        "ClientName": "ABC Corp",
        "BranchName": "Cairo Branch",
        "ProgramName": "Premium Plan",
        "StatusName": "Active",
        "Mobile": "01012345678"
      } 
    ]
  }
  ```
- **Errors:** Localized validation/conflict (via `HandleError`).
- **Notes:** Response uses PascalCase. Returns only specific fields: Id, BirthDate, Age, ClientName, BranchName, ProgramName, StatusName, Mobile.

### GET `/{id}`
- **Description:** Returns member details with normalized image URL.
- **Success:**
  ```json
  {
    "Id": 10,
    "Name": "Ahmed Ali",
    "NationalId": "29005151234567",
    "BirthDate": "1990-05-15",
    "Age": 33,
    "IsMale": true,
    "JobTitle": "Engineer",
    "Notes": "VIP Member",
    "PrivateNotes": "Internal notes",
    "MobileNumber": "01012345678",
    "Client": "ABC Corp",
    "Branch": "Cairo Branch",
    "Program": "Premium Plan",
    "CompanyCode": "EMP001",
    "Status": "Active",
    "VipStatus": "VIP",
    "ImageUrl": "https://api.mci.com/uploads/members/10.png",
    "ActivatedDate": "2024-01-01"
  }
  ```
- **Errors:** 404 `MemberNotFound` (localized).
- **Notes:** Does not include `Relation` field. Image URL is absolute.

### POST `/`
- **Body:** `MemberInfoCreateDto` (`multipart/form-data`, includes personal data, branch, VIP status, optional `MemberImage` <=5MB).
- **Success (201):** Created DTO (image URL absolute).
- **Errors:** 400 validation (phone/national ID/status), 409 conflict, 500 unexpected.

### PUT `/{id}`
- **Body:** `MemberInfoUpdateDto` (multipart). Allows partial updates plus image replacement/removal. Supports `StatusId` update.
- **Success:** Updated DTO.
- **Errors:** 404 / 400 / 409 depending on service result.

### PATCH `/{id}/status`
- **Description:** Updates member status by StatusId.
- **Body:** `MemberStatusUpdateDto` with `StatusId` (required, int).
- **Success (200):** `{ "message": "Member status updated" }`.
- **Errors:**
  - 404: Member not found
  - 400: Status not found or validation error

### DELETE `/{id}`
- **Success:** `{ "message": "Member deleted" }`.
- **Errors:** 404.

### GET `/clients`
- **Description:** Returns all clients for lookup (Id and Name only).
- **Success (200):**
  ```json
  [
    { "Id": 1, "Name": "ABC Corp" },
    { "Id": 2, "Name": "XYZ Ltd" }
  ]
  ```
- **Notes:** Names are localized (Arabic/English).

### GET `/statuses`
- **Description:** Returns all available statuses for lookup.
- **Success (200):**
  ```json
  [
    { "Id": 1, "Name": "Activated" },
    { "Id": 2, "Name": "Deactivated" }
  ]
  ```
- **Notes:** Names are localized.

### GET `/branches/{clientId}`
- **Description:** Returns all branches for a specific client.
- **Route Params:** `clientId` (int)
- **Success (200):**
  ```json
  [
    { "Id": 1, "Name": "Cairo Branch" },
    { "Id": 2, "Name": "Alexandria Branch" }
  ]
  ```
- **Notes:** Names are localized. Only returns non-deleted branches.

### GET `/programs/{branchId}`
- **Description:** Returns all programs used in a specific branch.
- **Route Params:** `branchId` (int)
- **Success (200):**
  ```json
  [
    { "Id": 1, "Name": "Premium Plan" },
    { "Id": 2, "Name": "Basic Plan" }
  ]
  ```
- **Notes:** Names are localized. Returns distinct programs from member policies in that branch.

### GET `/levels`
- **Description:** Returns all member levels for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "Premium" }, ... ]`
- **Notes:** Names are localized.

### GET `/vip-statuses`
- **Description:** Returns all VIP statuses for lookup.
- **Success (200):** `[ { "Id": 1, "Name": "VIP" }, ... ]`
- **Notes:** Names are localized.

- **Notes:** Uses `GetCurrentUserName()` for auditing where applicable; images converted to absolute URLs. Response uses PascalCase property names.

---

## 14. MemberPolicyInfoController (`api/{lang}/MemberPolicyInfo`)
Also `[Authorize]`. Manages member policy info records, including attachments.

### GET `/`
- **Description:** Returns all member policies.
- **Success:** `MemberPolicyDto[]` with image URLs normalized.
- **Errors:** Standard `HandleError` mapping (404/400/409/500).

### GET `/{id}`
- **Success:** Single policy DTO with absolute image path.
- **Errors:** 404 `MemberPolicyNotFound` (localized).

### POST `/`
- **Body:** `MemberPolicyCreateDto` (`multipart/form-data`). Includes references to member/policy/program IDs plus optional `ImageFile`.
- **Success (201):** Created DTO with image URL.
- **Errors:** 400 validation (invalid foreign keys), 409 conflicts, 404 missing dependencies.

### PUT `/{id}`
- **Body:** `MemberPolicyUpdateDto` (multipart) for partial updates.
- **Success:** Updated DTO.
- **Errors:** 404/400/409.

### DELETE `/{id}`
- **Success:** `{ "message": "Member policy deleted" }`.
- **Errors:** 404.

- **Notes:** Uses helper to prepend scheme/host to stored relative paths.

---

## 15. PolicyController (`api/{lang}/Policy`)
Inherits `BaseApiController` and marked `[Authorize]`; covers policy lifecycle.

### GET `/`
- **Description:** Returns all policies (with status, client, totals, etc.).
- **Success:** `PolicyReadDto[]`.
- **Errors:** Localized via `HandleError`.

### GET `/{id}`
- **Success:** Single policy DTO.
- **Errors:** 404 `PolicyNotFound`.

### POST `/`
- **Body:** `PolicyCreateDto` (JSON) with `ClientId`, `InsuranceCompanyId`, period dates, totals, etc.
- **Success (201):** Created DTO, `CreatedBy` set from token user.
- **Errors:** 400 validation (date ranges, duplicate active policy), 409 conflicts.

### PUT `/{id}`
- **Body:** `PolicyUpdateDto`.
- **Success:** Updated policy DTO.
- **Errors:** 404 not found, 400 validation/conflict.

### DELETE `/{id}`
- **Success:** `{ "message": "Policy deleted" }`.
- **Errors:** 404 not found.

- **Notes:** `HandleError` maps `ServiceErrorType` to correct HTTP status; controller reuses token user for auditing (`GetCurrentUserName`).

---

## 16. ProviderCategoriesController (`api/{lang}/ProviderCategories`)
Manages the master list of provider categories (localized names).

### GET `/`
- **Description:** Returns all provider categories with localized `Name` field based on `lang` route parameter.
- **Success:** `[ { "id": 1, "name": "مستشفى" } ]`.

### GET `/{id}`
- **Success:** `{ "id": 5, "name": "Clinic" }`.
- **Errors:** 404 `CategoryNotFound` (localized).

### POST `/`
- **Body:** `ProviderCategoryCreateDto` (Arabic/English names required, uniqueness enforced).
- **Success (201):**
  ```json
  {
    "message": "Category created successfully",
    "data": { "id": 12, "nameAr": "مركز", "nameEn": "Center" }
  }
  ```
- **Errors:** 400 validation/conflict (e.g., `CategoryNameExists`).

### PUT`/{id}`
- **Body:** `ProviderCategoryUpdateDto`.
- **Success:** `{ "message": "Category updated successfully" }`.
- **Errors:** 404 not found, 400 conflict.

### DELETE `/{id}`
- **Success:** `{ "message": "Category deleted successfully" }`.
- **Errors:** 404 not found.

- **Authorization:** Not explicitly set; follow global policies.

---

## 17. ProviderController (`api/{lang}/Provider`)
Full provider lifecycle plus nested price-list/discount management. `[Authorize]` on controller.

### GET `/`
- **Query:** `ProviderSearchFilterDto` (id, name, category, status, network class, page/pageSize, etc.).
- **Success:** `ProviderPagedResultDto` with absolute `ImageUrl`s.
- **Errors:** 400 validation, 404 if filter references missing data.

### GET `/{id}`
- **Success:** `ProviderDetailDto` with attachments mapped to absolute URLs.
- **Errors:** 404 `ProviderNotFound`.

### POST `/`
- **Body:** `ProviderCreateDto` (`multipart/form-data`, includes metadata, attachments, optional image files, nested financial data).
- **Success (201):** Created provider DTO.
- **Errors:** 400 validation, 409 duplicate VAT/commercial register.

### PUT `/{id}`
- **Body:** `ProviderUpdateDto` (multipart) for partial updates.
- **Success:** Updated provider detail.
- **Errors:** 404/400/409.

### PATCH `/{id}/toggle-active`
- **Description:** Flips provider active flag.
- **Success:** `{ "message": "Provider status updated" }`.
- **Errors:** 404 provider not found.

### DELETE `/{id}`
- **Description:** Soft-deletes provider.
- **Success:** `{ "message": "Provider deleted" }`.
- **Errors:** 404.

### PUT `/{id}/restore`
- **Description:** Restores a previously deleted provider.
- **Success:** `{ "message": "Provider restored" }`.

### Price Lists (`/{providerId}/price-lists`)
- `GET`: list price entries.
- `POST`: body `ProviderPriceListCreateDto` (service name, price). Success returns new entry.
- `PUT`/`DELETE`: update/remove entries by `priceListId`. Errors: 404 provider or price list missing.

### Discounts (`/{providerId}/discounts`)
- Similar structure using `ProviderDiscountCreateDto/UpdateDto`.
- Success returns updated list; errors include validation (percentage range) and not found.

- **Notes:** `EnhanceListFileUrls/EnhanceDetailFileUrls` convert stored paths to absolute URLs; `HandleError` maps `ServiceErrorType` to appropriate HTTP codes.

---

## 18. ProviderLocationsController (`api/{lang}/providers/{providerId}/locations`)
Protected nested resource for provider branches/locations.

### GET `/`
- **Description:** Lists locations for the given provider.
- **Success:** `ProviderLocationReadDto[]`.
- **Errors:** 404 provider not found.

### GET `/{id}`
- **Success:** Single location details.
- **Errors:** 404 location/provider not found.

### POST `/`
- **Body:** `ProviderLocationCreateDto` (branch info, contact data, allowed chronic flag, etc.).
- **Success (201):** Created location.
- **Errors:** 400 validation, 404 provider missing.

### PUT `/{id}`
- **Body:** `ProviderLocationUpdateDto`.
- **Success:** Updated location.
- **Errors:** 404 not found, 400 validation.

### DELETE `/{id}`
- **Success:** 204 No Content.
- **Errors:** 404 not found.

### PATCH `/{id}/toggle-chronic`
- **Description:** Toggles `AllowChronic` flag.
- **Success:** Updated location.
- **Errors:** 404 not found.

- **Notes:** Uses `HandleError` to return localized messages; inherits `[Authorize]` from `BaseApiController`.

---

## 19. RelationsController (`api/{lang}/Relations`)
Manages relationship types (e.g., spouse, child).

### GET `/`
- **Description:** Returns list of relations.
- **Success:** `RelationReadDto[]`.

### GET `/{id}`
- **Success:** Relation details.
- **Errors:** 404 not found.

### POST `/`
- **Body:** `RelationCreateDto` (localized names, ordering, etc.).
- **Success (201):** Created relation.
- **Errors:** 400 validation.

### PUT `/{id}`
- **Body:** `RelationUpdateDto`.
- **Success:** Updated relation.
- **Errors:** 404 not found, 400 validation.

### DELETE `/{id}`
- **Success:** `{ "message": "Relation deleted" }`.
- **Errors:** 404 not found.

---

## 20. WeatherForecastController (`api/[controller]`)
Default sample controller from ASP.NET template.

### GET `/`
- **Description:** Returns an array of 5 weather forecast samples.
- **Authorization:** None.
- **Success:**
  ```json
  [
    { "date": "2025-11-18", "temperatureC": 17, "summary": "Mild", "temperatureF": 63 },
    ...
  ]
  ```
- **Notes:** Used mainly for sanity checks; not localized.

---

## 21. ApprovalController (`api/{lang}/Approval`)
Manages approval records for claims. Requires authentication (`[Authorize]`).

### GET `/`
- **Description:** Retrieves paginated approvals with optional search.
- **Query Params:**
  - `page` (default 1)
  - `limit` (default 10)
  - `search` (optional): Search term
- **Success (200):**
  ```json
  {
    "TotalApprovals": 50,
    "CurrentPage": 1,
    "Limit": 10,
    "TotalPages": 5,
    "Data": [ { "Id": 1, "ClaimId": 5, "Status": "Approved", ... } ]
  }
  ```
- **Errors:** Standard error handling.

### GET `/{id}`
- **Description:** Returns approval by ID.
- **Success (200):** `ApprovalReadDto` object.
- **Errors:** 404 `ApprovalNotFound` (localized).

### POST `/`
- **Description:** Creates a new approval record.
- **Body:** `ApprovalCreateDto` (JSON).
- **Success (201):** Created approval DTO.
- **Errors:** 
  - 400: Validation errors
  - 401: Unauthorized (if employee ID cannot be determined)
- **Notes:** Uses current employee ID from token for auditing.

### PUT `/{id}`
- **Description:** Updates an approval record.
- **Body:** `ApprovalUpdateDto` (JSON).
- **Success (200):**
  ```json
  {
    "message": "Approval updated successfully",
    "data": { "Id": 1, ... }
  }
  ```
- **Errors:**
  - 404: Approval not found
  - 401: Unauthorized
  - 400: Validation errors

### DELETE `/{id}`
- **Description:** Deletes an approval record.
- **Success (200):** `{ "message": "Approval deleted successfully" }`.
- **Errors:**
  - 404: Approval not found
  - 401: Unauthorized

- **Authorization:** Requires Bearer token. Uses `GetCurrentEmployeeId()` from `BaseApiController` for auditing.

---

## Summary of Recent Updates

### ClientController Updates:
- Added search and filter functionality to GET `/` endpoint
- Added lookup endpoints: `/statuses`, `/types`, `/categories`, `/insurance-companies`, `/programs`, `/levels`, `/vip-statuses`
- Added Excel export endpoint: `/export/excel`
- Added status update endpoint: `PATCH /{id}/status`
- Added `LevelId` and `LevelName` to client details

### MemberInfoController Updates:
- Enhanced GET `/` with search and filter (by column or wildcard, status filter)
- Added member status update: `PATCH /{id}/status`
- Added lookup endpoints: `/clients`, `/statuses`, `/branches/{clientId}`, `/programs/{branchId}`, `/levels`, `/vip-statuses`
- Updated response structure to return only specific fields in list view
- Removed `Relation` field from responses
- Updated to use `StatusId` instead of `Status` string

### Response Format:
- All responses use **PascalCase** property names (configured globally in `Program.cs`)
- Image URLs are converted to absolute paths
- Localized error messages via `ILocalizationHelper`

---

## Next Steps
- Add authentication/authorization details per controller once policies/roles are finalized.
- Include sample curl commands or Postman collections for quick onboarding.
- Document any additional business rules or validation constraints.
