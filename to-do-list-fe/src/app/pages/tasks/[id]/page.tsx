"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";

interface ToDoList {
  id: string;
  name: string;
}

interface Task {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  isCompleted: boolean;
  isArchived: boolean;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
  toDoList: ToDoList;
}

export default function TaskDetails() {
  const { id } = useParams();
  const [task, setTask] = useState<Task | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/tasks/${id}`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch task.");
        return res.json();
      })
      .then((data) => {
        setTask(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error loading task:", err);
        setError("Failed to load task.");
        setLoading(false);
      });
  }, [id]);

  if (loading)
    return <p className="text-gray-500 text-center">Loading task...</p>;

  if (error)
    return <p className="text-red-500 text-center">{error}</p>;

  if (!task)
    return <p className="text-gray-500 text-center">Task not found.</p>;

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
      <h1 className="text-2xl font-bold mb-6 text-center">Task Details</h1>

      <div className="mb-4 space-y-2 text-gray-900">
        <p><strong>Title:</strong> {task.title}</p>
        <p><strong>Description:</strong> {task.description}</p>
        <p><strong>Due Date:</strong> {new Date(task.dueDate).toLocaleString()}</p>
        <p><strong>Created At:</strong> {new Date(task.createdAt).toLocaleString()}</p>
        <p><strong>Updated At:</strong> {new Date(task.updatedAt).toLocaleString()}</p>
        <p><strong>Status:</strong> {task.isCompleted ? "Completed" : "Pending"}</p>
        <p><strong>Archived:</strong> {task.isArchived ? "Yes" : "No"}</p>
        {task.completedAt && (
          <p><strong>Completed At:</strong> {new Date(task.completedAt).toLocaleString()}</p>
        )}
        {task.toDoList && (
          <p><strong>To-do List:</strong> {task.toDoList.name}</p>
        )}
      </div>

      <div className="text-center mt-4">
        <Link href="/pages/tasks" className="text-blue-500 hover:underline">
          Back to Task List
        </Link>
      </div>
    </div>
  );
}
