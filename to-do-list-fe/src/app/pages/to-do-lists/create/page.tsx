"use client";

import { useRouter } from "next/navigation";
import Link from "next/link";
import { API_BASE_URL } from "@/lib/api";
import { useEffect, useState } from "react";
import { useSearchParams } from 'next/navigation'

interface TodoListFormData {
  name: string;
  parentListId?: string | null;
}

interface TodoListOption {
  id: string;
  name: string;
  subLists?: TodoListOption[];
}


export default function CreateTodoList() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const parentId = searchParams.get("parentId");
  const [parentListName, setParentListName] = useState<string | null>(null);

  const [formValues, setFormValues] = useState<TodoListFormData>({
    name: "",
    parentListId: parentId ?? null,
  });

  const [availableLists, setAvailableLists] = useState<TodoListOption[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchLists = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/api/v1.0/todolists`);
        if (!res.ok) throw new Error("Failed to fetch to-do lists.");
        const data = await res.json();
        setAvailableLists(data);

        if (parentId) {
          const flatLists = flattenLists(data);
          const matchedParent = flatLists.find((list) => list.id === parentId);
          if (matchedParent) {
            setParentListName(matchedParent.name);
          }
        }
      } catch (err) {
        console.error("Error fetching to-do lists:", err);
      }
    };

    fetchLists();
  }, [parentId]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormValues({
      ...formValues,
      [name]: value === "" ? null : value,
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const response = await fetch(`${API_BASE_URL}/api/v1.0/todolists`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formValues),
      });

      if (!response.ok) {
        throw new Error("Failed to create to-do list.");
      }

      router.push("/pages/to-do-lists");
    } catch (err) {
      console.error("Error creating to-do list:", err);
      setError("An error occurred while creating the list.");
    } finally {
      setLoading(false);
    }
  };

  const renderListOptions = (
    lists: TodoListOption[],
    indent = ""
  ): React.ReactElement[] => {
    return lists.flatMap((list) => {
      const option = (
        <option key={list.id} value={list.id}>
          {indent + list.name}
        </option>
      );
      const subOptions: React.ReactElement[] = list.subLists
        ? renderListOptions(list.subLists, indent + "— ")
        : [];
      return [option, ...subOptions];
    });
  };

  const flattenLists = (lists: TodoListOption[]): TodoListOption[] => {
    return lists.flatMap((list) => [
      list,
      ...(list.subLists ? flattenLists(list.subLists) : []),
    ]);
  };

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
      <h1 className="text-2xl font-bold mb-6 text-center">Create To-do List</h1>

      {error && <p className="text-red-500 mb-4 text-center">{error}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Name Input */}
        <div>
          <label className="block text-gray-700 font-medium">List Name</label>
          <input
            type="text"
            name="name"
            value={formValues.name}
            onChange={handleChange}
            required
            className="w-full border border-gray-300 px-3 py-2 rounded-md focus:ring focus:ring-blue-300"
          />
        </div>

        {parentListName && (
          <p className="text-sm text-gray-600 italic mb-2">
            Creating a sub-list under: <strong>{parentListName}</strong>
          </p>
        )}

        <div>
          <label className="block text-gray-700 font-medium">Parent List</label>
          <select
            name="parentListId"
            value={formValues.parentListId ?? ""}
            onChange={handleChange}
            disabled={!!parentId}
            className="w-full border border-gray-300 px-3 py-2 rounded-md focus:ring focus:ring-blue-300"
          >
            <option value="">No Parent (Top-level list)</option>
            {renderListOptions(availableLists)}
          </select>
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition"
        >
          {loading ? "Creating..." : "Create List"}
        </button>
      </form>

      <div className="text-center mt-4">
        <Link href="/pages/to-do-lists" className="text-blue-500 hover:underline">
          Back to To-do Lists
        </Link>
      </div>
    </div>
  );
}
