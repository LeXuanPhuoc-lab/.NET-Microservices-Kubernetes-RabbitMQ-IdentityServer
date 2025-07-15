import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  logging: {
    fetches: {
      fullUrl: true,
    },
  },
  images: {
    remotePatterns: [{ protocol: "https", hostname: "cdn.pixabay.com" }],
  },
  output: "standalone",
};

export default nextConfig;
