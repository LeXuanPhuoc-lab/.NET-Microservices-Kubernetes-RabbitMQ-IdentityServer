'use client'

import React from 'react'
import { FaSearch } from 'react-icons/fa'
import { useParamsStore } from '../hooks/useParamsStore'

const Search = () => {
    const setParams = useParamsStore((state) => state.setParams);
    const searchValue = useParamsStore((state) => state.searchValue);
    const setSearchValue = useParamsStore((state) => state.setSearchValue);

    function onChange(event: any) {
        setSearchValue(event.target.value);
    }

    function search() {
        setParams({ searchValue: searchValue })
    }

    return (
        <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
            <input
                onKeyDown={(e: any) => {
                    if (e.key === 'Enter') search();
                }}
                onChange={onChange}
                value={searchValue}
                type='text'
                placeholder='Search for cars by make, model, or color'
                className='
                    flex-grow
                    pl-5
                    bg-transparent
                    border-transparent
                    focus:outline-none
                    focus:ring-0
                    focus:border-transparent
                '
            />

            <button type='submit' aria-label='search' onClick={search}>
                <FaSearch
                    size={34}
                    className='bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2' />
            </button>
        </div>
    )
}

export default Search