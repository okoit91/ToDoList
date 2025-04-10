"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useCreateFormPublic } from "@/hooks/useCretateFormPublic";
import { API_BASE_URL } from "@/lib/api";
import { useSearchParams } from "next/navigation";

interface TaskFormData {
    toDoListId: string;
    title: string;
    description: string;
    dueDate: string;
    isCompleted: boolean;
    completedAt: string | null;
}

interface ToDoList {
    id: string;
    name: string;
}

export default function CreateTask() {
    const router = useRouter();
    const searchParams = useSearchParams();
    const preselectedListId = searchParams.get("listId");
    const [toDoLists, setToDoLists] = useState<ToDoList[]>([]);
    const [listsLoading, setListsLoading] = useState(true);
    const [listsError, setListsError] = useState<string | null>(null);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/v1.0/todolists`)
            .then((res) => {
                if (!res.ok) throw new Error("Failed to load to-do lists.");
                return res.json();
            })
            .then((data) => setToDoLists(data))
            .catch(() => setListsError("Could not fetch to-do lists."))
            .finally(() => setListsLoading(false));
    }, []);

    const initialValues: TaskFormData = {
        toDoListId: preselectedListId || "",
        title: "",
        description: "",
        dueDate: "",
        isCompleted: false,
        completedAt: null,
    };

    const {
        formValues,
        setFormValues,
        handleChange,
        handleSubmit,
        error: formError,
        loading: formLoading,
    } = useCreateFormPublic<TaskFormData>(
        initialValues,
        `${API_BASE_URL}/api/v1.0/tasks`,
        "/pages/to-do-lists"
    );

    const handleListChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setFormValues({ ...formValues, toDoListId: e.target.value });
    };

    const handleSubmitDebug = async (e: React.FormEvent) => {
        e.preventDefault();

        setFormValues((prev) => ({
            ...prev,
            isCompleted: false,
            completedAt: null,
        }));

        try {
            await handleSubmit(e);
        } catch (error) {
            console.error("Error creating task:", error);
        }
    };

    if (listsLoading)
        return <p className="text-gray-500 text-center">Loading to-do lists...</p>;

    if (listsError)
        return <p className="text-red-500 text-center">{listsError}</p>;

    return (
        <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
            <h1 className="text-2xl font-bold mb-6 text-center">Create Task</h1>

            {formError && <p className="text-red-500 text-center mb-4">{formError}</p>}

            <form onSubmit={handleSubmitDebug} className="space-y-4">
                <div>
                    <label className="block text-gray-700 font-medium">To-do List</label>
                    {preselectedListId ? (
                        <div className="bg-gray-100 px-3 py-2 rounded-md border border-gray-300 text-gray-700">
                            Assigned To: {toDoLists.find((l) => l.id === preselectedListId)?.name || "Selected List"}
                        </div>
                    ) : (
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
                    )}
                </div>

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

                <button
                    type="submit"
                    disabled={formLoading}
                    className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition disabled:opacity-50"
                >
                    {formLoading ? "Creating..." : "Create Task"}
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
