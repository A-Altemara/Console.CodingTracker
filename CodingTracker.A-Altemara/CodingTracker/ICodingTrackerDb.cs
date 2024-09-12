using CodingTracker.A_Altemara.Models;

namespace CodingTracker.A_Altemara;

public interface ICodingTrackerDb<T> where T : IEntry
{
    /// <summary>
    /// Retrieves all records from the associated table.
    /// </summary>
    /// <returns>A list of all records from the database.</returns>
    List<T> GetAllRecords();

    /// <summary>
    /// Deletes a session record from the "CodeTrackerTable" table by its ID.
    /// </summary>
    /// <param name="id">The ID of the session record to delete.</param>
    /// <returns><c>true</c> if the record was deleted successfully; otherwise, <c>false</c>.</returns>
    bool Delete(string id);

    /// <summary>
    /// Adds a new session record to the "CodeTrackerTable" table.
    /// </summary>
    /// <param name="codingSession">The session record to add.</param>
    void Add(T entry);

    /// <summary>
    /// Updates an existing session record in the "CodeTrackerTable" table.
    /// </summary>
    /// <param name="codingSession">The session record to update.</param>
    /// <returns><c>true</c> if the record was updated successfully; otherwise, <c>false</c>.</returns>
    bool Update(T entry);
}