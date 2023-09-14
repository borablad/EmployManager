using ClosedXML.Excel;
using EmployManager.Models;
using System.Security.Cryptography;
using System.Text;
using static EmployManager.Models.Enums;

public static class ExcelExportHelper
{
    private static string CurrentDepartamentId { get => Preferences.Get(nameof(CurrentDepartamentId), ""); set => Preferences.Set(nameof(CurrentDepartamentId), value); }

    private static string CurrentOrganizationId { get => Preferences.Get(nameof(CurrentOrganizationId), ""); set => Preferences.Set(nameof(CurrentOrganizationId), value); }


    public static bool ImportMembersToExcel(List<Member> members, string filePath)
    {
        try
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Members");

                // Заголовки столбцов
                worksheet.Cell(1, 1).Value = "Имя";
                worksheet.Cell(1, 2).Value = "Фамилия";
                worksheet.Cell(1, 3).Value = "Отчество";
                worksheet.Cell(1, 4).Value = "Должность";
                worksheet.Cell(1, 5).Value = "Зарплата";
                worksheet.Cell(1, 6).Value = "Дополнительная информация";
                worksheet.Cell(1, 7).Value = "Логин для входа";
                worksheet.Cell(1, 8).Value = "Роль доступа в приложении";
                worksheet.Cell(1, 9).Value = "Пароль";

                int row = 2;

                foreach (var member in members)
                {
                    worksheet.Cell(row, 1).Value = member.FirstName;
                    worksheet.Cell(row, 2).Value = member.LastName;
                    worksheet.Cell(row, 3).Value = member.MiddleName;
                    worksheet.Cell(row, 4).Value = member.RoleName;
                    worksheet.Cell(row, 5).Value = member.Salary;
                    worksheet.Cell(row, 6).Value = string.Join(", ", member.Contacts.Select(c => $"{c.Title}: {c.Body}"));
                    worksheet.Cell(row, 7).Value = member.Username;
                    worksheet.Cell(row, 8).Value = member.RoleEnumString;

                    row++;
                }

                // Сохранить файл Excel
                workbook.SaveAs(filePath);
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static List<Member> ExportMembersFromExcel(string filePath)
    {
        try
        {
            var members = new List<Member>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.FirstOrDefault();

                if (worksheet != null)
                {
                    int rowCount = worksheet.RowsUsed().Count();

                    for (int row = 2; row <= rowCount; row++) // Начинаем с 2 строки, так как первая строка содержит заголовки
                    {
                        Member member;
                        try
                        {
                            member = new Member
                            {
                                FirstName = worksheet.Cell(row, 1).Value.ToString(),
                                LastName = worksheet.Cell(row, 2).Value.ToString(),
                                MiddleName = worksheet.Cell(row, 3).Value.ToString(),
                                RoleName = worksheet.Cell(row, 4).Value.ToString(),
                                Salary = Convert.ToDouble(worksheet.Cell(row, 5).Value.ToString()),
                                Username = worksheet.Cell(row, 7).Value.ToString(),
                                Role = GetRoleByVisualString(worksheet.Cell(row, 8).Value.ToString()),
                                Password = CreateHashPassword(worksheet.Cell(row, 9).Value.ToString()),
                                DepartamentId = CurrentDepartamentId,
                                OrganizationId = CurrentOrganizationId
                            };
                        }catch(Exception ex)
                        {
                            continue;
                        }

                        string contactsString = worksheet.Cell(row, 6).Value.ToString();
                        // Разбиваем строку контактов на отдельные контакты
                        string[] contactStrings = contactsString.Split(',');
                        foreach (var contactString in contactStrings)
                        {
                            string[] parts = contactString.Split(':');
                            if (parts.Length == 2)
                            {
                                var contact = new EmployManager.Models.Contacts
                                {
                                    Title = parts[0].Trim(),
                                    Body = parts[1].Trim()
                                };
                                member.Contacts.Add(contact);
                            }
                        }

                        members.Add(member);
                    }
                }
            }

            return members;
        }
        catch
        {
            return null;
        }
    }

    private static MembersRole GetRoleByVisualString(string value)
    {
        return value switch
        {
            "Администратор" => MembersRole.Admin,
            "Менеджер" => MembersRole.Manager,
            _ => MembersRole.User
        };
    }

    private static string CreateHashPassword(string password)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
