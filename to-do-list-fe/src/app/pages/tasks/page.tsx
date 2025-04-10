"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { API_BASE_URL } from "@/lib/api";

interface ToDoList {
  id: string;
  name: string;
}

interface Task {
  id: string;
  toDoListId: string;
  toDoList: ToDoList;
  title: string;
  description: string;
  dueDate: string;
  isCompleted: boolean;
  isArchived: boolean;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export default function Tasks() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/tasks`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch tasks.");
        return res.json();
      })
      .then((data) => {
        setTasks(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error loading tasks:", err);
        setError("Failed to load tasks.");
        setLoading(false);
      });
  }, []);

  if (loading)
    return <p className="text-gray-500 text-center">Loading tasks...</p>;

  if (error)
    return <p className="text-red-500 text-center">{error}</p>;

  return (
    <div className="max-w-3xl mx-auto mt-10 bg-white p-6 rounded-lg shadow-lg">
      <h1 className="text-2xl font-bold text-gray-900 text-center mb-6">
        Tasks
      </h1>

      <div className="text-center mb-4">
        <Link href="/pages/tasks/create" className="text-blue-500 hover:underline">
          Create New Task
        </Link>
      </div>

      <ul className="space-y-4">
        {tasks.filter(task => !task.isArchived).length > 0 ? (
          tasks
            .filter((task) => !task.isArchived)
            .map((task) => (
              <li
                key={task.id} className="border-b pb-4 flex justify-between items-start">
                <div>
                  <p className="text-lg font-semibold text-gray-800">{task.title}</p>
                  <p className="text-sm text-gray-600">{task.description}</p>
                  <p className="text-sm text-gray-500">
                    List: <span className="font-medium">{task.toDoList?.name}</span>
                  </p>
                  <p className="text-sm text-gray-500">
                    Due: {new Date(task.dueDate).toLocaleDateString()}
                  </p>
                  <p className={`text-sm font-medium ${task.isCompleted ? "text-green-600" : "text-yellow-600"}`}>
                    {task.isCompleted ? `Completed at ${new Date(task.completedAt!).toLocaleString()}` : "Pending"}
                  </p>
                </div>

                <div className="space-y-1 text-sm text-right">
                  <Link
                    href={`/pages/tasks/${task.id}`}
                    className="text-green-500 hover:underline"
                  >
                    Details
                  </Link>
                  <br />
                  <Link
                    href={`/pages/tasks/${task.id}/edit`}
                    className="text-blue-500 hover:underline"
                  >
                    Edit
                  </Link>
                  <br />
                  <Link
                    href={`/pages/tasks/${task.id}/delete`}
                    className="text-red-500 hover:underline"
                  >
                    Delete
                  </Link>
                </div>
              </li>
            ))
        ) : (
          <p className="text-gray-600 text-center">No tasks found.</p>
        )}
      </ul>
    </div>
  );
}
