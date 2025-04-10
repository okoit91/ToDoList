// app/layout.tsx
import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import Header from "@/components/nav/Header";
import Footer from "@/components/nav/Footer";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Todo List App",
  description: "A simple public todo list app",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={`${geistSans.className} flex flex-col min-h-screen`}>
        <div className="flex flex-col min-h-screen">
          <Header />
          <main className="flex-1 container mx-auto py-4">{children}</main>
          <Footer />
        </div>
      </body>
    </html>
  );
}
