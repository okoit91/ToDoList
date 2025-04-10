"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function Header() {
  const router = useRouter();

  return (
    <header className="bg-white shadow-md">
      <nav className="container mx-auto flex justify-between items-center py-4 px-6">
        
        <Link href="/" className="text-xl font-bold text-gray-900">
          To-Do App
        </Link>

        <div className="space-x-6 text-sm font-medium text-gray-700">
          <Link href="/pages/to-do-lists" className="hover:text-blue-600 transition">
            To-Do Lists
          </Link>
          <Link href="/pages/tasks" className="hover:text-blue-600 transition">
            Tasks
          </Link>
          <Link href="/pages/task-histories" className="hover:text-blue-600 transition">
            Task Histories
          </Link>
        </div>
      </nav>
    </header>
  );
}