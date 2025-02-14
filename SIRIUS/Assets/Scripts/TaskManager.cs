using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Manages tasks, their time since start, and their status
public class TaskManager : MonoBehaviour
{
    [SerializeField]
    private GameObject taskListObject;

    [SerializeField]
    private GameObject currentTaskObject;

    [SerializeField]
    private GameObject[] nextTaskObjects;

    private Dictionary<string, float> taskTime = new Dictionary<string, float>();

    // 0 = in progress, 1 = completed, -1 = paused
    private Dictionary<string, int> taskStatus = new Dictionary<string, int>();

    // Keeps track of the current task
    private string currentTask;

    // Task Order - using a LinkedList here to make it easier to remove the first element
    private LinkedList<string> taskOrder = new LinkedList<string>();

    void Start()
    {
        // Test tasks - can be replaced with a file read or other method
        AddTask("Task 1aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        AddTask("Task 2");
        AddTask("Task 3");
        AddTask("Task 4");
        AddTask("Task 5");
        AddTask("Task 6");
        AddTask("Task 7");
        AddTask("Task 8");
        AddTask("Task 9");
        AddTask("Task 10");

        // Set the first task as the current task - don't delete
        currentTask = taskOrder.First.Value;
        taskStatus[currentTask] = 0;
    }

    void Update()
    {
        // Update current task time
        if (taskStatus[currentTask] == 0) {
            taskTime[currentTask] += Time.deltaTime;
        }
        
        // Update task UI
        taskListObject.GetComponent<TMP_Text>().text = GetTasks();
        currentTaskObject.GetComponent<TMP_Text>().text = "Current Task:\n" + currentTask;

        // Updating next tasks Ui
        var nextTask = taskOrder.First.Next;

        for (int i = 0; i < nextTaskObjects.Length; i++)
        {
            nextTaskObjects[i].GetComponent<TMP_Text>().text = nextTask.Value;
            nextTask = nextTask.Next;
        }
    }

    // Add a task to the task list - add to taskTime, taskStatus, and taskOrder
    public void AddTask(string taskName)
    {
        taskTime.Add(taskName, -1);
        taskStatus.Add(taskName, -1);
        taskOrder.AddLast(taskName);
    }

    // Complete the current task - set status to 1, remove from taskOrder, and set the next task as the current task
    public void CompleteTask()
    {
        taskStatus[currentTask] = 1;
        taskOrder.RemoveFirst();
        currentTask = taskOrder.First.Value;
        taskStatus[currentTask] = 0;
    }

    // Pause the current task
    public void PauseTask(string taskName)
    {
        taskStatus[taskName] = -1;
    }

    // Format the tasks for the task list
    string GetTasks() {
        string tasks = "";
        foreach (var task in taskStatus)
        {
            string status = "";
            if (task.Value == 0) {
                status = "In Progress";
            } else if (task.Value == 1) {
                status = "Completed";
            } else {
                status = "Paused";
            }
            tasks += "• " + task.Key + " - " + status + "\n";
        }
        return tasks;
    }

    // Get the current task time
    string GetCurrentTaskTime() {
        return taskTime[currentTask].ToString();
    }
}
