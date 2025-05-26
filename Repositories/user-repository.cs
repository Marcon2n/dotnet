public class UserRepository
{
    private readonly List<User> _users = new List<User>();

    public bool IsUsernameAvailable(string username)
    {
        return !_users.Any(u => u.Username == username);
    }

    public void AddUser(User user)
    {
        _users.Add(user);
        Console.WriteLine($"User added: {user.Username}");
    }

    // Method to get all users
    public List<User> GetUsers()
    {
        return _users;
    }

    // Method to find a specific user by username
    public User GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }


}
