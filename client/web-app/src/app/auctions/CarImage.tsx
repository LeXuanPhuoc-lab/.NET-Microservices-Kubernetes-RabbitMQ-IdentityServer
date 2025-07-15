'use client'

import React, { useState } from 'react'
import Image from 'next/image';

type Props = {
    imageUrl: string
}

export default function CarImage({imageUrl}: Props) {
    const [isLoading, setIsLoading] = useState(true);

    return (
        <Image 
            src={imageUrl}
            alt='Image of car'
            fill // Filled up the container with the image 
            priority
            // Format the image within the available area
            className= {`
                object-cover group-hover:opacity-65 duration-700 ease-in-out
                ${isLoading 
                    ? 'grayscale blur-2xl scale-110'
                    : 'grayscale-0 blur-0 scale-100'}
                `} 
            sizes='(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw'
            onLoad={() => setIsLoading(false)}
        />
  )
}
