"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { API_BASE_URL } from "@/lib/api";
import Link from "next/link";

interface TodoListFormData {
  name: string;
  createdAt: string;
  parentListId: string | null;
  subLists: string[];
}

export default function EditTodoList() {
  const { id } = useParams();
  const router = useRouter();

  const [formValues, setFormValues] = useState<TodoListFormData>({
    name: "",
    createdAt: "",
    parentListId: null,
    subLists: [],
  });

  const [loading, setLoading] = useState(true);
  const [formLoading, setFormLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch existing to-do list
  useEffect(() => {
    fetch(`${API_BASE_URL}/api/v1.0/todolists/${id}`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch to-do list.");
        return res.json();
      })
      .then((data) => {
        setFormValues({
          name: data.name || "",
          createdAt: data.createdAt,
          parentListId: data.parentListId ?? null,
          subLists: data.subLists ?? [],
        });
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error:", err);
        setError("Could not load to-do list.");
        setLoading(false);
      });
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormValues((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    setError(null);

    try {
      const payload = {
        id,
        ...formValues,
      };

      const response = await fetch(`${API_BASE_URL}/api/v1.0/todolists/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const result = await response.json();
        throw new Error(result?.message || "Update failed");
      }

      router.push("/pages/to-do-lists");
    } catch (err: any) {
      console.error("Edit error:", err);
      setError("Failed to update to-do list.");
    } finally {
      setFormLoading(false);
    }
  };

  if (loading) return <p className="text-gray-700 text-center">Loading...</p>;
  if (error) return <p className="text-red-500 text-center">{error}</p>;

  return (
    <div className="max-w-lg mx-auto p-6 bg-white shadow-md rounded-md mt-10">
      <h1 className="text-2xl font-bold mb-6 text-center">Edit To-do List</h1>

      {error && <p className="text-red-500 text-center">{error}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Name field */}
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

        <button
          type="submit"
          disabled={formLoading}
          className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition disabled:opacity-50"
        >
          {formLoading ? "Updating..." : "Update List"}
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
