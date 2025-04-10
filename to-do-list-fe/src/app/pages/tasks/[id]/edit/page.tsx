"use client";

import { useEffect, useState } from "react";
import { useRouter, useParams } from "next/navigation";
import Link from "next/link";
import { API_BASE_URL } from "@/lib/api";

interface ToDoList {
  id: string;
  name: string;
  createdAt: string;
  parentListId: string;
  parentList: string;
  subLists: string[];
}

interface TaskFormData {
  id: string;
  toDoListId: string;
  toDoList: ToDoList;
  title: string;
  description: string;
  dueDate: string;
  isCompleted: boolean;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export default function EditTask() {
  const router = useRouter();
  const params = useParams();
  const taskId = params?.id as string;

  const [toDoLists, setToDoLists] = useState<ToDoList[]>([]);
  const [formValues, setFormValues] = useState<TaskFormData | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [updating, setUpdating] = useState(false);

  // Fetch all to-do lists
  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/todolists`)
      .then((res) => res.json())
      .then(setToDoLists)
      .catch(() => setError("Failed to fetch to-do lists"));
  }, []);

  // Fetch the task by ID
  useEffect(() => {
    if (!taskId) return;

    fetch(`${API_BASE_URL}/api/v1.0/tasks/${taskId}`)
      .then((res) => {
        if (!res.ok) throw new Error("Task not found");
        return res.json();
      })
      .then((data) => {
        setFormValues({
          ...data,
          dueDate: data.dueDate?.slice(0, 16), // Trim for datetime-local
        });
        setLoading(false);
      })
      .catch(() => {
        setError("Failed to load task.");
        setLoading(false);
      });
  }, [taskId]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormValues((prev) => prev ? { ...prev, [name]: value } : null);
  };

  const handleListChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const selectedId = e.target.value;
    const selectedList = toDoLists.find((list) => list.id === selectedId);
    if (!selectedList) return;

    setFormValues((prev) => prev ? {
      ...prev,
      toDoListId: selectedId,
      toDoList: selectedList
    } : null);
  };

  const handleCheckboxChange = () => {
    const now = new Date().toISOString();
    setFormValues((prev) =>
      prev
        ? {
            ...prev,
            isCompleted: !prev.isCompleted,
            completedAt: !prev.isCompleted ? now : null,
          }
        : null
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formValues) return;
  
    setUpdating(true);
    setError(null);
  
    const toUtcIsoString = (value: string | null) =>
      value ? new Date(value).toISOString() : null;
  
    const payload = {
      ...formValues,
      dueDate: toUtcIsoString(formValues.dueDate),
      completedAt: toUtcIsoString(formValues.completedAt),
      createdAt: toUtcIsoString(formValues.createdAt),
      updatedAt: new Date().toISOString(), // ensure updatedAt is set now
    };
  
    try {
      const res = await fetch(`${API_BASE_URL}/api/v1.0/tasks/${formValues.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
  
      if (!res.ok) {
        const msg = await res.text();
        throw new Error(`Failed to update task: ${msg}`);
      }
  
      router.push("/pages/tasks");
    } catch (err) {
      console.error(err);
      setError("Something went wrong while updating.");
    } finally {
      setUpdating(false);
    }
  };

  if (loading || !formValues)
    return <p className="text-gray-500 text-center">Loading task...</p>;

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
      <h1 className="text-2xl font-bold mb-6 text-center">Edit Task</h1>

      {error && <p className="text-red-500 text-center mb-4">{error}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* To-do List Select */}
        <div>
          <label className="block text-gray-700 font-medium">To-do List</label>
          <select
            name="toDoListId"
            value={formValues.toDoListId}
            onChange={handleListChange}
            required
            className="w-full border border-gray-300 px-3 py-2 rounded-md focus:ring focus:ring-blue-300"
          >
            <option value="">Select a list</option>
            {toDoLists.map((list) => (
              <option key={list.id} value={list.id}>
                {list.name}
              </option>
            ))}
          </select>
        </div>

        {/* Title */}
        <div>
          <label className="block text-gray-700 font-medium">Title</label>
          <input
            type="text"
            name="title"
            value={formValues.title}
            onChange={handleChange}
            required
            className="w-full border border-gray-300 px-3 py-2 rounded-md focus:ring focus:ring-blue-300"
          />
        </div>

        {/* Description */}
        <div>
          <label className="block text-gray-700 font-medium">Description</label>
          <textarea
            name="description"
            value={formValues.description}
            onChange={handleChange}
            required
            className="w-full border border-gray-300 px-3 py-2 rounded-md focus:ring focus:ring-blue-300"
          />
        </div>

        {/* Due Date */}
        <div>
          <label className="block text-gray-700 font-medium">Due Date</label>
          <input
            type="datetime-local"
            name="dueDate"
            value={formValues.dueDate}
            onChange={handleChange}
            required
            className="w-full border border-gray-300 px-3 py-2 rounded-md focus:ring focus:ring-blue-300"
          />
        </div>

        {/* Completed */}
        <div className="flex items-center">
          <input
            type="checkbox"
            checked={formValues.isCompleted}
            onChange={handleCheckboxChange}
            className="mr-2"
          />
          <label className="text-gray-700">Mark as Completed</label>
        </div>

        <button
          type="submit"
          disabled={updating}
          className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition disabled:opacity-50"
        >
          {updating ? "Updating..." : "Update Task"}
        </button>
      </form>

      <div className="text-center mt-4">
        <Link href="/pages/tasks" className="text-blue-500 hover:underline">
          Back to Task List
        </Link>
      </div>
    </div>
  );
}
